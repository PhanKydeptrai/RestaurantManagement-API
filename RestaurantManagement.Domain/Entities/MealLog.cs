namespace RestaurantManagement.Domain.Entities;

public class MealLog 
{
    public Ulid MealLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogDetails { get; set; }
    public DateTime LogDate { get; set; }
    public User? User { get; set; }
}
