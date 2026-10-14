using DishManagement.Models;

namespace DishManagement.ViewModels;

public class DishListQuery
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Status { get; set; }      // all | available | unavailable
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Sort { get; set; }        // price_asc | price_desc | time_asc
}

public class DishListViewModel
{
    public DishListQuery Query { get; set; } = new();
    public List<Dish_BIT240069> Dishes { get; set; } = new();
    public List<DishCategory_BIT240069> Categories { get; set; } = new();
    public bool PriceRangeInvalid { get; set; }
}

public class CategoryListItem
{
    public DishCategory_BIT240069 Category { get; set; } = null!;
    public int DishCount { get; set; }
}
