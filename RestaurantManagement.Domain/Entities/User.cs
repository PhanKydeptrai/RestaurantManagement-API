using System.Net;

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
    public byte[]? UserImage { get; set; }
    public string? Gender { get; set; } 
    public ICollection<Notification>? Notifications { get; set; }
    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<SystemLog>? SystemLogs { get; set; }
    public ICollection<BookingChangeLog>? BookingChangeLogs { get; set; }
    public ICollection<OrderChangeLog>? OrderChangeLogs { get; set; }
    public ICollection<EmailVerificationToken>? EmailVerificationTokens { get; set; }
}
