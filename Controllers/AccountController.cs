using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    private const string AdminUser = "theworldspellcard@gmail.com";
    private const string AdminPass = "5432github";

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl ?? Url.Action("Index", "Admin");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (username == AdminUser && password == AdminPass)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            var id = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(id));

            return Redirect(returnUrl ?? "/Admin");
        }

        ModelState.AddModelError("", "ユーザー名またはパスワードが違います。");
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denied() => View();
}

