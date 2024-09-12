namespace RestaurantManagement.Domain.Entities;

public class OrderDetail
{
    public Guid OrderDetailId { get; set; }
    public Guid OrderId { get; set; }
    public Guid MealId { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; }
    public Meal? Meal { get; set; }
    public Order? Order { get; set; }
}
