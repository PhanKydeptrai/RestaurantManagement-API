namespace RestaurantManagement.Domain.Entities;

public class Notification
{
    public Ulid NotificationId { get; set; }
    public string Paragraph { get; set; }
    public DateTime Time { get; set; }
    public Ulid UserId { get; set; }
    public string Status { get; set; }
    public User? User { get; set; }
}
