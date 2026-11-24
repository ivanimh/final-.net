using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;

namespace ShuttleTiket.Controllers;

// CRUD Rute Antarkota (admin)
[Authorize]
public class RuteController : Controller
{
    private readonly AppDbContext _db;
    public RuteController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var data = await _db.Rute
            .OrderBy(r => r.KotaAsal).ThenBy(r => r.KotaTujuan)
            .ToListAsync();
        return View(data);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Tambah Rute";
        return View("Form", new Rute());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Rute rute)
    {
        ValidasiKota(rute);
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Tambah Rute";
            return View("Form", rute);
        }

        _db.Rute.Add(rute);
        return await SimpanAsync("Rute berhasil ditambahkan.", "Tambah Rute", rute);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rute = await _db.Rute.FindAsync(id);
        if (rute == null) return NotFound();

        ViewData["Title"] = "Edit Rute";
        return View("Form", rute);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Rute rute)
    {
        if (id != rute.Id) return BadRequest();
        ValidasiKota(rute);
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Rute";
            return View("Form", rute);
        }

        _db.Rute.Update(rute);
        return await SimpanAsync("Rute berhasil diperbarui.", "Edit Rute", rute);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rute = await _db.Rute.FindAsync(id);
        if (rute == null) return NotFound();

        try
        {
            _db.Rute.Remove(rute);
            await _db.SaveChangesAsync();
            TempData["Sukses"] = "Rute berhasil dihapus.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Rute tidak bisa dihapus karena sudah dipakai di jadwal.";
        }
        return RedirectToAction(nameof(Index));
    }

    private void ValidasiKota(Rute rute)
    {
        if (string.Equals(rute.KotaAsal?.Trim(), rute.KotaTujuan?.Trim(), StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(Rute.KotaTujuan), "Kota tujuan tidak boleh sama dengan kota asal.");
    }

    private async Task<IActionResult> SimpanAsync(string pesan, string judul, Rute rute)
    {
        try
        {
            await _db.SaveChangesAsync();
            TempData["Sukses"] = pesan;
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Rute dengan kota asal & tujuan tersebut sudah ada.");
            ViewData["Title"] = judul;
            return View("Form", rute);
        }
    }
}
