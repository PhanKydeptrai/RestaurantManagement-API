namespace RestaurantManagement.Domain.Entities;

public class Meal
{
    public Guid MealId { get; set; }    
    public string MealName { get; set; }
    public decimal Price { get; set; }
    public byte[]? Image { get; set; }
    public string? Description { get; set; }
    public string MealStatus { get; set; }
    public string SellStatus { get; set; }
    //Liên kết với Category
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    //Liên kết với OrderDetail
    public ICollection<OrderDetail>? OrderDetails { get; set; }
}
