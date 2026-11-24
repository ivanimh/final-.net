using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;
using ShuttleTiket.Models.ViewModels;

namespace ShuttleTiket.Controllers;

// Pencarian jadwal oleh penumpang
// Membaca SQL VIEW `vw_jadwal_shuttle`
public class PencarianController : Controller
{
    private readonly AppDbContext _db;
    public PencarianController(AppDbContext db) => _db = db;

    // GET /Pencarian?Asal=Bandung&Tujuan=Jakarta&Tanggal=2026-11-24&SudahCari=true
    public async Task<IActionResult> Index(CariJadwalViewModel vm)
    {
        await IsiDaftarKotaAsync(_db, vm);

        if (vm.SudahCari)
        {
            var query = _db.VwJadwalShuttle                        
                .Where(j => j.StatusJadwal == Jadwal.StatusAktif && j.Tanggal == vm.Tanggal.Date);

            if (!string.IsNullOrWhiteSpace(vm.Asal))
                query = query.Where(j => j.KotaAsal == vm.Asal);
            if (!string.IsNullOrWhiteSpace(vm.Tujuan))
                query = query.Where(j => j.KotaTujuan == vm.Tujuan);

            var hasil = await query.OrderBy(j => j.JamBerangkat).ToListAsync();

            // Sembunyikan jadwal yang jamnya sudah lewat
            vm.Hasil = hasil.Where(j => j.WaktuBerangkat > DateTime.Now).ToList();

            // Hasil kosong, tawarkan jadwal terdekat untuk rute yang sama
            if (vm.Hasil.Count == 0)
                vm.JadwalLain = (await AmbilJadwalMendatangAsync(_db, 30, vm.Asal, vm.Tujuan)).Take(6).ToList();
        }
        else
        {
            // Belum mencari, tampilkan jadwal 3 hari ke depan sebagai daftar awal
            vm.JadwalLain = await AmbilJadwalMendatangAsync(_db, 3);
        }

        return View(vm);
    }

    // Jadwal AKTIF yang belum berangkat, dari hari ini sampai ke depan. Dipakai juga oleh HomeController
    public static async Task<List<VwJadwalShuttle>> AmbilJadwalMendatangAsync(
        AppDbContext db, int hari, string? asal = null, string? tujuan = null)
    {
        var hariIni = DateTime.Today;
        var batas = hariIni.AddDays(hari);

        var query = db.VwJadwalShuttle                                
            .Where(j => j.StatusJadwal == Jadwal.StatusAktif && j.Tanggal >= hariIni && j.Tanggal < batas);

        if (!string.IsNullOrWhiteSpace(asal))
            query = query.Where(j => j.KotaAsal == asal);
        if (!string.IsNullOrWhiteSpace(tujuan))
            query = query.Where(j => j.KotaTujuan == tujuan);

        var data = await query.OrderBy(j => j.Tanggal).ThenBy(j => j.JamBerangkat).ToListAsync();
        return data.Where(j => j.WaktuBerangkat > DateTime.Now).ToList();
    }

    // Dipakai juga oleh HomeController untuk form pencarian di beranda
    public static async Task IsiDaftarKotaAsync(AppDbContext db, CariJadwalViewModel vm)
    {
        vm.DaftarKotaAsal = await db.VwJadwalShuttle
            .Select(j => j.KotaAsal).Distinct().OrderBy(k => k).ToListAsync();
        vm.DaftarKotaTujuan = await db.VwJadwalShuttle
            .Select(j => j.KotaTujuan).Distinct().OrderBy(k => k).ToListAsync();
    }
}
