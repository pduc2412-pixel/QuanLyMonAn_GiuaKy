using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DishManagement.Models;

[Table("BIT240069_DishCategories")]
public class DishCategory_BIT240069
{
    [Key]
    [Column("BIT240069_Id")]
    public int Id { get; set; }

    [Column("BIT240069_Name")]
    [Required(ErrorMessage = "val_name_required")]
    [StringLength(100, ErrorMessage = "val_too_long")]
    public string Name { get; set; } = "";

    [Column("BIT240069_Description")]
    [StringLength(500, ErrorMessage = "val_too_long")]
    public string? Description { get; set; }

    [ValidateNever]
    public List<Dish_BIT240069> Dishes { get; set; } = new();
}
