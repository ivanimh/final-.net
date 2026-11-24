using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShuttleTiket.Data;
using ShuttleTiket.Models;
using ShuttleTiket.Models.ViewModels;

namespace ShuttleTiket.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new CariJadwalViewModel { Tanggal = DateTime.Today.AddDays(1) };
        await PencarianController.IsiDaftarKotaAsync(_db, vm);

        var jadwal = await PencarianController.AmbilJadwalMendatangAsync(_db, 7);
        vm.RuteTersedia = jadwal
            .GroupBy(j => new { j.KotaAsal, j.KotaTujuan })
            .Select(g => new RingkasanRute
            {
                KotaAsal = g.Key.KotaAsal,
                KotaTujuan = g.Key.KotaTujuan,
                Harga = g.First().Harga,
                BerangkatBerikutnya = g.First().WaktuBerangkat,   
                JumlahJadwal = g.Count()
            })
            .OrderBy(r => r.KotaAsal).ThenBy(r => r.KotaTujuan)
            .ToList();

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
