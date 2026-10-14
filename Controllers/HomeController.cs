using DishManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace DishManagement.Controllers;

public class HomeController : AppController
{
    public HomeController(LangService l) : base(l) { }

    // Đổi ngôn ngữ, quay lại đúng trang (kèm query string => giữ điều kiện lọc)
    public IActionResult SetLanguage(string lang, string? returnUrl)
    {
        Response.Cookies.Append(LangService.CookieName, lang == "vi" ? "vi" : "en", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/",
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction("Index", "Dishes");
    }

    public IActionResult StatusPage(int id)   // id = mã lỗi HTTP (route /Home/StatusPage/{code})
    {
        return View("NotFound", id == 404 ? "err_page_nf" : "err_generic");
    }

    public IActionResult Error() => View();
}
