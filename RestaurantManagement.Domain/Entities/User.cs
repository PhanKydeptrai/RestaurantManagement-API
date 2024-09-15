namespace RestaurantManagement.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    public byte[]? UserImage { get; set; }

    //property
    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<Notification>? Notifications { get; set; }

}
