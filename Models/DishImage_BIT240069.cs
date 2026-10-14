using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DishManagement.Models;

[Table("BIT240069_DishImages")]
public class DishImage_BIT240069
{
    [Key]
    [Column("BIT240069_Id")]
    public int Id { get; set; }

    [Column("BIT240069_ImageUrl")]
    [Required(ErrorMessage = "val_image_required")]
    [StringLength(1000, ErrorMessage = "val_too_long")]
    public string ImageUrl { get; set; } = "";

    [Column("BIT240069_IsThumbnail")]
    public bool IsThumbnail { get; set; }

    [Column("BIT240069_DishId")]
    public int DishId { get; set; }

    [ValidateNever]
    public Dish_BIT240069? Dish { get; set; }
}
