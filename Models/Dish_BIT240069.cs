using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DishManagement.Models;

[Table("BIT240069_Dishes")]
public class Dish_BIT240069
{
    public const int QuickMinutes = 15;

    [Key]
    [Column("BIT240069_Id")]
    public int Id { get; set; }

    [Column("BIT240069_Name")]
    [Required(ErrorMessage = "val_name_required")]
    [StringLength(200, ErrorMessage = "val_too_long")]
    public string Name { get; set; } = "";

    [Column("BIT240069_Price", TypeName = "decimal(18,2)")]
    [Range(0.01, 1000000000, ErrorMessage = "val_price")]
    public decimal Price { get; set; }

    [Column("BIT240069_PreparationTime")]
    [Range(1, 10000, ErrorMessage = "val_prep")]
    public int PreparationTime { get; set; }

    [Column("BIT240069_IsAvailable")]
    public bool IsAvailable { get; set; } = true;

    [Column("BIT240069_Description")]
    [StringLength(1000, ErrorMessage = "val_too_long")]
    public string? Description { get; set; }

    [Column("BIT240069_DishCategoryId")]
    [Range(1, int.MaxValue, ErrorMessage = "val_category_required")]
    public int DishCategoryId { get; set; }

    [ValidateNever]
    public DishCategory_BIT240069? Category { get; set; }

    [ValidateNever]
    public List<DishImage_BIT240069> Images { get; set; } = new();

    // ---- Tính khi hiển thị, KHÔNG lưu cột trong database ----
    [NotMapped]
    public string PrepClassKey => PreparationTime <= QuickMinutes ? "prep_quick" : "prep_regular";

    [NotMapped]
    public DishImage_BIT240069? ThumbnailOrFirst =>
        Images.FirstOrDefault(i => i.IsThumbnail) ?? Images.OrderBy(i => i.Id).FirstOrDefault();
}
