using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;

namespace ShuttleTiket.Controllers;

// Kelola Jadwal Keberangkatan (admin)
[Authorize]
public class JadwalController : Controller
{
    private readonly AppDbContext _db;
    public JadwalController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(DateTime? tanggal)
    {
        var query = _db.VwJadwalShuttle.AsQueryable();

        if (tanggal.HasValue)
            query = query.Where(j => j.Tanggal == tanggal.Value.Date);
        else
            query = query.Where(j => j.Tanggal >= DateTime.Today);

        ViewBag.Tanggal = tanggal?.ToString("yyyy-MM-dd");
        var data = await query
            .OrderBy(j => j.Tanggal).ThenBy(j => j.JamBerangkat)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Create()
    {
        await IsiDropdownAsync();
        ViewData["Title"] = "Tambah Jadwal";
        return View("Form", new Jadwal());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Jadwal jadwal)
    {
        if (!ModelState.IsValid)
            return await TampilkanFormAsync(jadwal, "Tambah Jadwal");

        _db.Jadwal.Add(jadwal);
        return await SimpanAsync(jadwal, "Jadwal berhasil ditambahkan.", "Tambah Jadwal");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var jadwal = await _db.Jadwal.FindAsync(id);
        if (jadwal == null) return NotFound();
        return await TampilkanFormAsync(jadwal, "Edit Jadwal");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Jadwal jadwal)
    {
        if (id != jadwal.Id) return BadRequest();
        if (!ModelState.IsValid)
            return await TampilkanFormAsync(jadwal, "Edit Jadwal");

        _db.Jadwal.Update(jadwal);
        return await SimpanAsync(jadwal, "Jadwal berhasil diperbarui.", "Edit Jadwal");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var jadwal = await _db.Jadwal.FindAsync(id);
        if (jadwal == null) return NotFound();

        try
        {
            _db.Jadwal.Remove(jadwal);
            await _db.SaveChangesAsync();
            TempData["Sukses"] = "Jadwal berhasil dihapus.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Jadwal sudah memiliki pemesanan. Ubah statusnya menjadi BATAL saja.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ----- helper -----
    private async Task IsiDropdownAsync()
    {
        var rute = await _db.Rute.OrderBy(r => r.KotaAsal).ThenBy(r => r.KotaTujuan).ToListAsync();
        var armada = await _db.Armada.OrderBy(a => a.KodeArmada).ToListAsync();

        ViewBag.RuteList = new SelectList(rute, nameof(Rute.Id), nameof(Rute.NamaRute));
        ViewBag.ArmadaList = new SelectList(
            armada.Select(a => new { a.Id, Nama = $"{a.KodeArmada} - {a.NamaArmada} ({a.Kapasitas} kursi)" }),
            "Id", "Nama");
    }

    private async Task<IActionResult> TampilkanFormAsync(Jadwal jadwal, string judul)
    {
        await IsiDropdownAsync();
        ViewData["Title"] = judul;
        return View("Form", jadwal);
    }

    private async Task<IActionResult> SimpanAsync(Jadwal jadwal, string pesan, string judul)
    {
        try
        {
            await _db.SaveChangesAsync();
            TempData["Sukses"] = pesan;
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Armada tersebut sudah punya jadwal di tanggal & jam yang sama.");
            return await TampilkanFormAsync(jadwal, judul);
        }
    }
}
