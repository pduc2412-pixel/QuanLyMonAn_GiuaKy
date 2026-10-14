using DishManagement.Data;
using DishManagement.Models;
using DishManagement.Services;
using DishManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Controllers;

public class DishCategoriesController : AppController
{
    private readonly AppDbContext _db;
    public DishCategoriesController(AppDbContext db, LangService l) : base(l) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.DishCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItem { Category = c, DishCount = c.Dishes.Count })
            .ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new DishCategory_BIT240069());

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,Description")] DishCategory_BIT240069 category)
    {
        category.Name = category.Name?.Trim() ?? "";
        if (!ModelState.IsValid)
        {
            LocalizeModelState();
            return View(category);
        }
        _db.DishCategories.Add(category);
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_cat_created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cat = await _db.DishCategories.FindAsync(id);
        if (cat == null) return NotFoundPage("err_cat_nf");
        return View(cat);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Description")] DishCategory_BIT240069 input)
    {
        var cat = await _db.DishCategories.FindAsync(id);
        if (cat == null) return NotFoundPage("err_cat_nf");

        input.Id = id;
        input.Name = input.Name?.Trim() ?? "";
        if (!ModelState.IsValid)
        {
            LocalizeModelState();
            return View(input);
        }
        cat.Name = input.Name;
        cat.Description = input.Description;
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_cat_updated";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.DishCategories.FindAsync(id);
        if (cat == null) return NotFoundPage("err_cat_nf");

        // Ràng buộc nghiệp vụ: không xóa loại món đang có món sử dụng
        if (await _db.Dishes.AnyAsync(d => d.DishCategoryId == id))
        {
            TempData["Error"] = "err_cat_in_use";
            return RedirectToAction(nameof(Index));
        }

        _db.DishCategories.Remove(cat);
        await _db.SaveChangesAsync();
        TempData["Success"] = "msg_cat_deleted";
        return RedirectToAction(nameof(Index));
    }
}
