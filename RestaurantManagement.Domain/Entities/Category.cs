using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain;

public class Category
{
    public Guid CategoryId { get; set; }    
    public string CategoryName { get; set; }
    public string CategoryStatus { get; set; }
    public string Desciption { get; set; }
    // Liên kết với Meal    
    public ICollection<Meal> Meals { get; set; }
}
