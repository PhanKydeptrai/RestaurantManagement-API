

namespace RestaurantManagement.Domain.Entities;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public Guid UserId { get; set; }    
    public string EmployeeStatus { get; set; }
    public string Role { get; set; }
    public User? User { get; set; }


}
