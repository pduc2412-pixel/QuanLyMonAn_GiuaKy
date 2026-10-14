using DishManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<DishCategory_BIT240069> DishCategories => Set<DishCategory_BIT240069>();
    public DbSet<Dish_BIT240069> Dishes => Set<Dish_BIT240069>();
    public DbSet<DishImage_BIT240069> DishImages => Set<DishImage_BIT240069>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // 1 DishCategory - n Dish (không cho xóa loại đang có món ở tầng DB)
        b.Entity<Dish_BIT240069>()
            .HasOne(d => d.Category)
            .WithMany(c => c.Dishes)
            .HasForeignKey(d => d.DishCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1 Dish - n DishImage
        b.Entity<DishImage_BIT240069>()
            .HasOne(i => i.Dish)
            .WithMany(d => d.Images)
            .HasForeignKey(i => i.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tên món không trùng trong cùng một loại (ràng buộc ở cả DB)
        b.Entity<Dish_BIT240069>()
            .HasIndex(d => new { d.DishCategoryId, d.Name })
            .IsUnique();
    }
}
