using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;

namespace ShuttleTiket.Controllers;

[Authorize]
public class ArmadaController : Controller
{
    private readonly AppDbContext _db;
    public ArmadaController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var data = await _db.Armada.OrderBy(a => a.KodeArmada).ToListAsync();
        return View(data);
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Tambah Armada";
        return View("Form", new Armada());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Armada armada)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Tambah Armada";
            return View("Form", armada);
        }

        _db.Armada.Add(armada);
        return await SimpanAsync("Armada berhasil ditambahkan.", "Tambah Armada", armada);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var armada = await _db.Armada.FindAsync(id);
        if (armada == null) return NotFound();

        ViewData["Title"] = "Edit Armada";
        return View("Form", armada);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Armada armada)
    {
        if (id != armada.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Armada";
            return View("Form", armada);
        }

        _db.Armada.Update(armada);
        return await SimpanAsync("Armada berhasil diperbarui.", "Edit Armada", armada);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var armada = await _db.Armada.FindAsync(id);
        if (armada == null) return NotFound();

        try
        {
            _db.Armada.Remove(armada);
            await _db.SaveChangesAsync();
            TempData["Sukses"] = "Armada berhasil dihapus.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Armada tidak bisa dihapus karena sudah dipakai di jadwal.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> SimpanAsync(string pesan, string judul, Armada armada)
    {
        try
        {
            await _db.SaveChangesAsync();
            TempData["Sukses"] = pesan;
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Kode armada atau plat nomor sudah terdaftar.");
            ViewData["Title"] = judul;
            return View("Form", armada);
        }
    }
}
