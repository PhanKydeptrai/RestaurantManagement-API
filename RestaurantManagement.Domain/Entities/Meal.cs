namespace RestaurantManagement.Domain.Entities;

public class Meal
{
    public Ulid MealId { get; set; }
    public string MealName { get; set; }
    public decimal Price { get; set; }
    public byte[]? Image { get; set; }
    public string? Description { get; set; }
    public string SellStatus { get; set; }
    public string MealStatus { get; set; }
    public Ulid CategoryId { get; set; }
    public ICollection<OrderDetail>? OrderDetails { get; set; }
    public Category? Category { get; set; }
}
