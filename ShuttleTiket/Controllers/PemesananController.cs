using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;
using ShuttleTiket.Models.ViewModels;

namespace ShuttleTiket.Controllers;

// Pemesanan tiket, pembayaran (simulasi), e-ticket, dan daftar pemesanan untuk admin.
public class PemesananController : Controller
{
    private const int MaksKursiPerPesanan = 5;
    private static readonly string[] MetodeValid = { "TRANSFER", "QRIS", "TUNAI" };

    private readonly AppDbContext _db;
    public PemesananController(AppDbContext db) => _db = db;

    // PENUMPANG

    // GET -> form data penumpang + pilih kursi
    public async Task<IActionResult> Pesan(int id)
    {
        var jadwal = await _db.VwJadwalShuttle.FirstOrDefaultAsync(j => j.JadwalId == id);
        if (jadwal == null) return NotFound();

        if (!BisaDipesan(jadwal))
        {
            TempData["Error"] = "Jadwal ini sudah tidak bisa dipesan (penuh, dibatalkan, atau sudah berangkat).";
            return RedirectToAction("Index", "Pencarian");
        }

        var vm = new PesanTiketViewModel
        {
            JadwalId = id,
            Jadwal = jadwal,
            KursiTerisi = await AmbilKursiTerisiAsync(id)
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Pesan(PesanTiketViewModel vm)
    {
        var jadwal = await _db.VwJadwalShuttle.FirstOrDefaultAsync(j => j.JadwalId == vm.JadwalId);
        if (jadwal == null) return NotFound();
        vm.Jadwal = jadwal;

        vm.KursiDipilih = vm.KursiDipilih.Distinct()
            .Where(k => k >= 1 && k <= jadwal.Kapasitas)
            .OrderBy(k => k).ToList();

        if (!BisaDipesan(jadwal))
            ModelState.AddModelError(string.Empty, "Jadwal ini sudah tidak bisa dipesan.");
        if (vm.KursiDipilih.Count == 0)
            ModelState.AddModelError(nameof(vm.KursiDipilih), "Pilih minimal 1 kursi.");
        if (vm.KursiDipilih.Count > MaksKursiPerPesanan)
            ModelState.AddModelError(nameof(vm.KursiDipilih), $"Maksimal {MaksKursiPerPesanan} kursi per pemesanan.");

        if (!ModelState.IsValid)
        {
            vm.KursiTerisi = await AmbilKursiTerisiAsync(vm.JadwalId);
            return View(vm);
        }

        // Transaksi + kunci baris jadwal supaya dua orang yang memesan bersamaan tidak mendapat kursi yang sama.
        await using var trx = await _db.Database.BeginTransactionAsync();
        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT id FROM jadwal WHERE id = {vm.JadwalId} FOR UPDATE");

        var terisi = await AmbilKursiTerisiAsync(vm.JadwalId);
        var bentrok = vm.KursiDipilih.Intersect(terisi).ToList();
        if (bentrok.Count > 0)
        {
            await trx.RollbackAsync();
            ModelState.AddModelError(nameof(vm.KursiDipilih),
                $"Kursi {string.Join(", ", bentrok)} baru saja dipesan orang lain. Silakan pilih kursi lain.");
            vm.KursiTerisi = terisi;
            vm.KursiDipilih = vm.KursiDipilih.Except(bentrok).ToList();
            return View(vm);
        }

        var pemesanan = new Pemesanan
        {
            KodeBooking = await BuatKodeBookingAsync(),
            JadwalId = vm.JadwalId,
            NamaPenumpang = vm.NamaPenumpang.Trim(),
            NoHp = vm.NoHp.Trim(),
            Email = string.IsNullOrWhiteSpace(vm.Email) ? null : vm.Email.Trim(),
            JumlahTiket = vm.KursiDipilih.Count,
            TotalHarga = jadwal.Harga * vm.KursiDipilih.Count,
            StatusPembayaran = Pemesanan.StatusMenunggu,
            TanggalPesan = DateTime.Now,
            Kursi = vm.KursiDipilih.Select(k => new PemesananKursi { NomorKursi = k }).ToList()
        };

        _db.Pemesanan.Add(pemesanan);
        await _db.SaveChangesAsync();
        await trx.CommitAsync();

        TempData["Sukses"] = $"Pemesanan berhasil! Simpan kode booking Anda: {pemesanan.KodeBooking}";
        return RedirectToAction(nameof(Detail), new { kode = pemesanan.KodeBooking });
    }

    // GET -> status + form pembayaran
    public async Task<IActionResult> Detail(string kode)
    {
        var data = await CariBookingAsync(kode);                     // <-- SQL VIEW 2
        if (data == null) return NotFound();
        return View(data);
    }

    // POST: simulasi pembayaran oleh penumpang
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Bayar(string kode, string metode)
    {
        var p = await _db.Pemesanan.FirstOrDefaultAsync(x => x.KodeBooking == kode);
        if (p == null) return NotFound();

        if (!MetodeValid.Contains(metode))
            TempData["Error"] = "Pilih metode pembayaran.";
        else if (p.StatusPembayaran != Pemesanan.StatusMenunggu)
            TempData["Error"] = "Pemesanan ini tidak dalam status menunggu pembayaran.";
        else
        {
            p.StatusPembayaran = Pemesanan.StatusLunas;
            p.MetodePembayaran = metode;
            p.TanggalBayar = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["Sukses"] = "Pembayaran berhasil. E-ticket sudah bisa dicetak.";
        }
        return RedirectToAction(nameof(Detail), new { kode });
    }

    // POST: penumpang membatalkan pesanan yang belum dibayar
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Batal(string kode)
    {
        var p = await _db.Pemesanan.FirstOrDefaultAsync(x => x.KodeBooking == kode);
        if (p == null) return NotFound();

        if (p.StatusPembayaran == Pemesanan.StatusMenunggu)
        {
            p.StatusPembayaran = Pemesanan.StatusBatal;
            await _db.SaveChangesAsync();
            TempData["Sukses"] = "Pemesanan dibatalkan.";
        }
        else
        {
            TempData["Error"] = "Hanya pemesanan yang belum dibayar yang bisa dibatalkan.";
        }
        return RedirectToAction(nameof(Detail), new { kode });
    }

    // GET -> cari booking berdasarkan kode
    public async Task<IActionResult> Cek(string? kode)
    {
        if (!string.IsNullOrWhiteSpace(kode))
        {
            var data = await CariBookingAsync(kode);
            if (data != null)
                return RedirectToAction(nameof(Detail), new { kode = data.KodeBooking });

            ViewBag.Pesan = $"Kode booking \"{kode}\" tidak ditemukan.";
        }
        ViewBag.Kode = kode;
        return View();
    }

    // GET -> e-ticket siap cetak (hanya LUNAS)
    public async Task<IActionResult> ETiket(string kode)
    {
        var data = await CariBookingAsync(kode);                     // <-- SQL VIEW 2
        if (data == null) return NotFound();

        if (data.StatusPembayaran != Pemesanan.StatusLunas)
        {
            TempData["Error"] = "E-ticket hanya tersedia untuk pemesanan yang sudah LUNAS.";
            return RedirectToAction(nameof(Detail), new { kode });
        }
        return View(data);
    }

    // ADMIN

    // GET /Pemesanan?status=MENUNGGU&cari=
    [Authorize]   // khusus admin
    public async Task<IActionResult> Index(string? status, string? cari)
    {
        var query = _db.VwDetailPemesanan.AsQueryable();             

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(p => p.StatusPembayaran == status);
        if (!string.IsNullOrWhiteSpace(cari))
            query = query.Where(p => p.KodeBooking.Contains(cari) || p.NamaPenumpang.Contains(cari));

        ViewBag.Status = status;
        ViewBag.Cari = cari;
        var data = await query.OrderByDescending(p => p.TanggalPesan).ToListAsync();
        return View(data);
    }

    // POST: admin mengonfirmasi pembayaran
    [Authorize, HttpPost, ValidateAntiForgeryToken]  
    public async Task<IActionResult> Konfirmasi(int id)
    {
        var p = await _db.Pemesanan.FindAsync(id);
        if (p == null) return NotFound();

        if (p.StatusPembayaran == Pemesanan.StatusMenunggu)
        {
            p.StatusPembayaran = Pemesanan.StatusLunas;
            p.MetodePembayaran ??= "TUNAI";
            p.TanggalBayar = DateTime.Now;
            await _db.SaveChangesAsync();
            TempData["Sukses"] = $"Pembayaran {p.KodeBooking} dikonfirmasi LUNAS.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: admin membatalkan pemesanan
    [Authorize, HttpPost, ValidateAntiForgeryToken]   
    public async Task<IActionResult> Batalkan(int id)
    {
        var p = await _db.Pemesanan.FindAsync(id);
        if (p == null) return NotFound();

        if (p.StatusPembayaran != Pemesanan.StatusBatal)
        {
            p.StatusPembayaran = Pemesanan.StatusBatal;
            await _db.SaveChangesAsync();
            TempData["Sukses"] = $"Pemesanan {p.KodeBooking} dibatalkan.";
        }
        return RedirectToAction(nameof(Index));
    }

    //  HELPER

    private Task<VwDetailPemesanan?> CariBookingAsync(string kode)
    {
        kode = (kode ?? string.Empty).Trim().ToUpper();
        return _db.VwDetailPemesanan.FirstOrDefaultAsync(p => p.KodeBooking == kode);
    }

    // Kursi yang sudah dipesan pada suatu jadwal
    private Task<List<int>> AmbilKursiTerisiAsync(int jadwalId)
    {
        return _db.PemesananKursi
            .Where(k => k.Pemesanan!.JadwalId == jadwalId
                     && k.Pemesanan.StatusPembayaran != Pemesanan.StatusBatal)
            .Select(k => k.NomorKursi)
            .ToListAsync();
    }

    private static bool BisaDipesan(VwJadwalShuttle j) =>
        j.StatusJadwal == Jadwal.StatusAktif && j.SisaKursi > 0 && j.WaktuBerangkat > DateTime.Now;

    // Format: SHT + 8 karakter
    private async Task<string> BuatKodeBookingAsync()
    {
        const string karakter = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        string kode;
        do
        {
            kode = "SHT" + RandomNumberGenerator.GetString(karakter, 8);
        } while (await _db.Pemesanan.AnyAsync(p => p.KodeBooking == kode));
        return kode;
    }
}
