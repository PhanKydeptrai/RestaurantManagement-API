namespace RestaurantManagement.Domain.Entities;

public class OrderDetail
{
    public Ulid OrderDetailId { get; set; }
    public Ulid OrderId { get; set; }
    public Ulid MealId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public decimal UnitPrice { get; set; }
    public Order? Order { get; set; }
    public Meal? Meal { get; set; }
}
