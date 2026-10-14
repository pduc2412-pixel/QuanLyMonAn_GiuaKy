using DishManagement.Data;
using DishManagement.Models;
using DishManagement.Services;
using DishManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Controllers;

public class DishesController : AppController
{
    private readonly AppDbContext _db;
    public DishesController(AppDbContext db, LangService l) : base(l) => _db = db;

    // ===================== LIST + SEARCH / FILTER / SORT =====================
    public async Task<IActionResult> Index([FromQuery] DishListQuery q)
    {
        // Khoảng giá không hợp lệ: nhập sai định dạng, âm, hoặc min > max
        bool priceInvalid = false;
        foreach (var key in new[] { "MinPrice", "MaxPrice" })
        {
            if (ModelState.TryGetValue(key, out var e) && e.Errors.Count > 0)
            {
                priceInvalid = true;
                ModelState.Remove(key);
            }
        }
        if (q.MinPrice < 0 || q.MaxPrice < 0) priceInvalid = true;
        if (q.MinPrice.HasValue && q.MaxPrice.HasValue && q.MinPrice > q.MaxPrice) priceInvalid = true;
        if (priceInvalid) { q.MinPrice = null; q.MaxPrice = null; }

        q.Search = q.Search?.Trim();

        // Toàn bộ điều kiện được dịch sang SQL (IQueryable) - không lọc bằng vòng lặp
        IQueryable<Dish_BIT240069> query = _db.Dishes.AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Images);

        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(d => d.Name.Contains(q.Search));
        if (q.CategoryId.HasValue)
            query = query.Where(d => d.DishCategoryId == q.CategoryId.Value);
        if (q.Status == "available")
            query = query.Where(d => d.IsAvailable);
        else if (q.Status == "unavailable")
            query = query.Where(d => !d.IsAvailable);
        if (q.MinPrice.HasValue)
            query = query.Where(d => d.Price >= q.MinPrice.Value);
        if (q.MaxPrice.HasValue)
            query = query.Where(d => d.Price <= q.MaxPrice.Value);

        query = q.Sort switch
        {
            "price_asc" => query.OrderBy(d => d.Price).ThenBy(d => d.Name),
            "price_desc" => query.OrderByDescending(d => d.Price).ThenBy(d => d.Name),
            "time_asc" => query.OrderBy(d => d.PreparationTime).ThenBy(d => d.Name),
            _ => query.OrderBy(d => d.Name)
        };

        var vm = new DishListViewModel
        {
            Query = q,
            Dishes = await query.ToListAsync(),
            Categories = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
            PriceRangeInvalid = priceInvalid
        };
        return View(vm);
    }

    // ===================== DETAILS =====================
    public async Task<IActionResult> Details(int id)
    {
        var dish = await _db.Dishes.AsNoTracking()
            .Include(d => d.Category)
            .Include(d => d.Images)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (dish == null) return NotFoundPage("err_dish_nf");

        dish.Images = dish.Images.OrderByDescending(i => i.IsThumbnail).ThenBy(i => i.Id).ToList();
        return View(dish);
    }

    // ===================== CREATE =====================
    public async Task<IActionResult> Create()
    {
        await LoadCategories(null);
        return View(new Dish_BIT240069 { IsAvailable = true });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240069 dish)
    {
        await ValidateDish(dish, 0);
        if (!ModelState.IsValid)
        {
            LocalizeModelState();
            await LoadCategories(dish.DishCategoryId);
            return View(dish);
        }

        _db.Dishes.Add(dish);
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_dish_created";
        return RedirectToAction(nameof(Details), new { id = dish.Id });
    }

    // ===================== EDIT =====================
    public async Task<IActionResult> Edit(int id)
    {
        var dish = await _db.Dishes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (dish == null) return NotFoundPage("err_dish_nf");
        await LoadCategories(dish.DishCategoryId);
        return View(dish);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id,
        [Bind("Name,Price,PreparationTime,IsAvailable,Description,DishCategoryId")] Dish_BIT240069 input)
    {
        var dish = await _db.Dishes.FindAsync(id);
        if (dish == null) return NotFoundPage("err_dish_nf");

        input.Id = id;
        await ValidateDish(input, id);
        if (!ModelState.IsValid)
        {
            LocalizeModelState();
            await LoadCategories(input.DishCategoryId);
            return View(input);
        }

        dish.Name = input.Name;
        dish.Price = input.Price;
        dish.PreparationTime = input.PreparationTime;
        dish.IsAvailable = input.IsAvailable;
        dish.Description = input.Description;
        dish.DishCategoryId = input.DishCategoryId;
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_dish_updated";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ===================== DELETE =====================
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var dish = await _db.Dishes.FindAsync(id);
        if (dish == null) return NotFoundPage("err_dish_nf");
        _db.Dishes.Remove(dish);   // ảnh bị xóa theo (cascade)
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_dish_deleted";
        return RedirectToAction(nameof(Index));
    }

    // ===================== IMAGES =====================
    [HttpPost]
    public async Task<IActionResult> AddImage(int id, string? imageUrl, bool isThumbnail)
    {
        if (!await _db.Dishes.AnyAsync(d => d.Id == id)) return NotFoundPage("err_dish_nf");

        imageUrl = imageUrl?.Trim();
        if (string.IsNullOrEmpty(imageUrl))
        {
            TempData["Error"] = "val_image_required";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (imageUrl.Length > 1000 || !IsValidImageUrl(imageUrl))
        {
            TempData["Error"] = imageUrl.Length > 1000 ? "val_too_long" : "val_image_url";
            return RedirectToAction(nameof(Details), new { id });
        }

        var hasThumb = await _db.DishImages.AnyAsync(i => i.DishId == id && i.IsThumbnail);
        var image = new DishImage_BIT240069
        {
            DishId = id,
            ImageUrl = imageUrl,
            IsThumbnail = isThumbnail || !hasThumb   // ảnh đầu tiên tự là ảnh đại diện
        };

        if (image.IsThumbnail) await ClearThumbnails(id);
        _db.DishImages.Add(image);
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_image_added";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SetThumbnail(int id)   // id = Id của ảnh
    {
        var image = await _db.DishImages.FindAsync(id);
        if (image == null) return NotFoundPage("err_image_nf");

        await ClearThumbnails(image.DishId);    // mỗi món tối đa 1 ảnh đại diện
        image.IsThumbnail = true;
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_thumb_set";
        return RedirectToAction(nameof(Details), new { id = image.DishId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int id)    // id = Id của ảnh
    {
        var image = await _db.DishImages.FindAsync(id);
        if (image == null) return NotFoundPage("err_image_nf");

        var dishId = image.DishId;
        _db.DishImages.Remove(image);
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_image_deleted";
        return RedirectToAction(nameof(Details), new { id = dishId });
    }

    // ===================== HELPERS =====================
    private async Task ClearThumbnails(int dishId)
    {
        var current = await _db.DishImages.Where(i => i.DishId == dishId && i.IsThumbnail).ToListAsync();
        foreach (var i in current) i.IsThumbnail = false;
    }

    private static bool IsValidImageUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var u))
            return u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps;
        return url.StartsWith("/") && !url.StartsWith("//");
    }

    private async Task LoadCategories(int? selectedId)
    {
        var cats = await _db.DishCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        ViewBag.Categories = new SelectList(cats.Select(c => new { c.Id, Name = L.Cat(c.Name) }), "Id", "Name", selectedId);
    }

    // Validation nghiệp vụ: loại món tồn tại + tên không trùng trong cùng loại
    private async Task ValidateDish(Dish_BIT240069 dish, int excludeId)
    {
        dish.Name = dish.Name?.Trim() ?? "";

        if (dish.DishCategoryId > 0)
        {
            if (!await _db.DishCategories.AnyAsync(c => c.Id == dish.DishCategoryId))
            {
                ModelState.AddModelError(nameof(dish.DishCategoryId), "val_category_nf");
            }
            else if (dish.Name.Length > 0 &&
                     await _db.Dishes.AnyAsync(d => d.DishCategoryId == dish.DishCategoryId
                                                    && d.Name == dish.Name && d.Id != excludeId))
            {
                ModelState.AddModelError(nameof(dish.Name), "val_name_dup");
            }
        }
    }
}
