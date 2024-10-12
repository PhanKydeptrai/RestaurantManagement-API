namespace RestaurantManagement.Domain.Entities;

public class EmailVerificationToken
{
    public Ulid EmailVerificationTokenId { get; set; }
    public Ulid UserId { get; set; }
    public string? Temporary { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiredDate { get; set; } 
    public User? User { get; set; }
}
