namespace RestaurantManagement.Domain.Entities;

public class Category
{
    public Guid CategoryId { get; set; }    
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Desciption { get; set; }
    // Liên kết với Meal    
    public ICollection<Meal> Meals { get; set; }
}
