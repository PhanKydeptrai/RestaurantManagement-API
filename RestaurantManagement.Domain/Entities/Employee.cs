namespace RestaurantManagement.Domain.Entities;

public class Employee
{
    public Ulid EmployeeId { get; set; }
    public Ulid UserId { get; set; }
    public string EmployeeStatus { get; set; }
    public string Role { get; set; }
    public User? User { get; set; }
}
