using DishManagement.Models;

namespace DishManagement.Data;

public static class SeedData
{
    public static void Init(AppDbContext db)
    {
        if (db.DishCategories.Any()) return;

        var main = new DishCategory_BIT240069 { Name = "Món chính", Description = "Main dishes" };
        var starter = new DishCategory_BIT240069 { Name = "Khai vị", Description = "Appetizers" };
        var drink = new DishCategory_BIT240069 { Name = "Đồ uống", Description = "Drinks" };
        db.DishCategories.AddRange(main, starter, drink);
        db.SaveChanges();

        string Img(string seed) => $"https://picsum.photos/seed/{seed}/600/400";

        var dishes = new List<Dish_BIT240069>
        {
            new() { Name = "Phở bò", Price = 55000, PreparationTime = 12, IsAvailable = true, Description = "Phở bò truyền thống", DishCategoryId = main.Id,
                    Images = { new() { ImageUrl = Img("pho1"), IsThumbnail = true }, new() { ImageUrl = Img("pho2") } } },
            new() { Name = "Bún chả", Price = 60000, PreparationTime = 20, IsAvailable = true, Description = "Bún chả Hà Nội", DishCategoryId = main.Id,
                    Images = { new() { ImageUrl = Img("buncha"), IsThumbnail = true } } },
            new() { Name = "Cơm tấm sườn", Price = 50000, PreparationTime = 15, IsAvailable = true, Description = "Cơm tấm sườn nướng", DishCategoryId = main.Id },
            new() { Name = "Gỏi cuốn", Price = 35000, PreparationTime = 10, IsAvailable = true, Description = "Gỏi cuốn tôm thịt", DishCategoryId = starter.Id,
                    Images = { new() { ImageUrl = Img("goicuon1"), IsThumbnail = true }, new() { ImageUrl = Img("goicuon2") }, new() { ImageUrl = Img("goicuon3") } } },
            new() { Name = "Nem rán", Price = 40000, PreparationTime = 25, IsAvailable = true, Description = "Nem rán giòn", DishCategoryId = starter.Id },
            new() { Name = "Cà phê sữa đá", Price = 25000, PreparationTime = 5, IsAvailable = true, Description = "Cà phê phin", DishCategoryId = drink.Id,
                    Images = { new() { ImageUrl = Img("caphe"), IsThumbnail = true } } },
            new() { Name = "Trà đào", Price = 30000, PreparationTime = 8, IsAvailable = false, Description = "Tạm ngừng bán", DishCategoryId = drink.Id },
        };
        db.Dishes.AddRange(dishes);
        db.SaveChanges();
    }
}
