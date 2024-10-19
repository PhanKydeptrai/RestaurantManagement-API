namespace RestaurantManagement.Domain.Entities;

public class Category
{
    public Ulid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryStatus { get; set; }
    public ICollection<Meal>? Meals { get; set; }
}
