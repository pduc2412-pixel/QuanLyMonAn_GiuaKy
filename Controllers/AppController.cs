using DishManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace DishManagement.Controllers;

public abstract class AppController : Controller
{
    protected readonly LangService L;
    protected AppController(LangService l) => L = l;

    private static readonly Dictionary<string, string> Fallback = new()
    {
        ["Name"] = "val_name_required",
        ["Price"] = "val_price",
        ["PreparationTime"] = "val_prep",
        ["DishCategoryId"] = "val_category_required",
        ["ImageUrl"] = "val_image_required",
    };

    /// <summary>Trang 404 theo ngôn ngữ hiện tại (message là KEY trong LangService).</summary>
    protected IActionResult NotFoundPage(string messageKey)
    {
        Response.StatusCode = 404;
        return View("NotFound", messageKey);
    }

    /// <summary>
    /// Dịch toàn bộ lỗi trong ModelState sang ngôn ngữ hiện tại.
    /// Lỗi tự định nghĩa dùng KEY (val_...), lỗi mặc định của framework (vd nhập chữ vào ô số)
    /// được thay bằng thông báo theo trường.
    /// </summary>
    protected void LocalizeModelState()
    {
        foreach (var kv in ModelState.ToList())
        {
            var entry = kv.Value;
            if (entry == null || entry.Errors.Count == 0) continue;

            var messages = entry.Errors.Select(e => e.ErrorMessage).ToList();
            entry.Errors.Clear();
            var translated = messages.Select(m =>
            {
                string key = !string.IsNullOrEmpty(m) && m.StartsWith("val_")
                    ? m
                    : (Fallback.TryGetValue(kv.Key, out var f) ? f : "val_invalid");
                return L.T(key);
            }).Distinct();
            foreach (var t in translated) entry.Errors.Add(t);
        }
    }
}
