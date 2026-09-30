using System.Security.Claims;
using HomeAwayFromHome.DTOs;
using HomeAwayFromHome.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace HomeAwayFromHome.Controllers;

public class AccountController : Controller
{
    private readonly AuthApiService _auth;
    public AccountController(AuthApiService auth) => _auth = auth;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginApiRequest { });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginApiRequest model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var result = await _auth.LoginAsync(model);

        if (!result.Success || result.Response == null) 
        { 
            ModelState.AddModelError("", result.Error); 
            return View(model); 
        }

        var a = result.Response;
        HttpContext.Session.SetString("JwtToken", a.Token);

        var claims = new List<Claim> 
        { 
            new(ClaimTypes.NameIdentifier, a.UserId), new(ClaimTypes.Name, a.FullName), new(ClaimTypes.Email, a.Email) 
        };

        claims.AddRange(a.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties 
        { 
            IsPersistent = true, ExpiresUtc = a.ExpiresAt 
        });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) 
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterApiRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterApiRequest model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var result = await _auth.RegisterAsync(model);

        if (!result.Success) 
        { 
            ModelState.AddModelError("", result.Error); 
            return View(model); 
        }

        TempData["Success"] = "Registration successful. You can now log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
