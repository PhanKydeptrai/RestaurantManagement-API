namespace RestaurantManagement.Domain.Entities;

public class Notification
{
    public Guid NotificationId { get; set; }
    public string Pagragraph { get; set; }
    public DateTime Time { get; set; }
    public string Status { get; set; } // Xem hay chưa
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
