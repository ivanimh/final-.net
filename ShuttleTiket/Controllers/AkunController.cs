using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShuttleTiket.Data;
using ShuttleTiket.Models;

namespace ShuttleTiket.Controllers;

// login & logout admin
public class AkunController : Controller
{
    private static readonly PasswordHasher<Admin> Hasher = new();
    private readonly AppDbContext _db;
    public AkunController(AppDbContext db) => _db = db;

    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
            return KembaliKe(returnUrl);

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var admin = await _db.Admin.FirstOrDefaultAsync(a => a.Username == vm.Username.Trim());
        var cocok = admin != null &&
                    Hasher.VerifyHashedPassword(admin, admin.PasswordHash, vm.Password) != PasswordVerificationResult.Failed;

        if (!cocok)
        {
            ModelState.AddModelError(string.Empty, "Username atau password salah.");
            vm.Password = string.Empty;
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin!.Id.ToString()),
            new(ClaimTypes.Name, admin.Username),
            new("NamaLengkap", admin.NamaLengkap)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        TempData["Sukses"] = $"Masuk sebagai {admin.NamaLengkap}.";
        return KembaliKe(vm.ReturnUrl);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Sukses"] = "Anda sudah keluar.";
        return RedirectToAction("Index", "Home");
    }

    private IActionResult KembaliKe(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Jadwal");
}
