using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;

namespace ShuttleTiket.Controllers;

[Authorize]
public class LaporanController : Controller
{
    private readonly AppDbContext _db;
    public LaporanController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var data = await _db.VwRekapPenjualanRute
            .OrderByDescending(r => r.TotalPendapatan)
            .ToListAsync();
        return View(data);
    }
}
