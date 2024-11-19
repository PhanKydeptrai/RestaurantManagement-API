namespace RestaurantManagement.Domain.Entities;

public class User
{
    public Ulid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public string Status { get; set; }
    public string? Email { get; set; }
    public string? ImageUrl { get; set; }
    public string? Gender { get; set; }
    public ICollection<Notification>? Notifications { get; set; }
    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<EmailVerificationToken>? EmailVerificationTokens { get; set; }
    //Logs
    public ICollection<UserLog>? UserLogs { get; set; }
    public ICollection<OrderLog>? OrderLogs { get; set; }
    public ICollection<BillLog>? BillLogs { get; set; }
    public ICollection<TableLog>? TableLogs { get; set; }
    public ICollection<MealLog>? MealLogs { get; set; }
    public ICollection<CategoryLog>? CategoryLogs { get; set; }
    public ICollection<CustomerLog>? CustomerLogs { get; set; }
    public ICollection<EmployeeLog>? EmployeeLogs { get; set; }
    public ICollection<TableTypeLog>? TableTypeLogs { get; set; }
    public ICollection<BookingLog>? BookingLogs { get; set; }
}
