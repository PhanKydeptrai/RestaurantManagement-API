namespace RestaurantManagement.Domain.Entities;

public class EmployeeLog
{
    public Ulid EmployeeLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogDetails { get; set; }
    public DateTime LogDate { get; set; }
    public User? User { get; set; }
}
