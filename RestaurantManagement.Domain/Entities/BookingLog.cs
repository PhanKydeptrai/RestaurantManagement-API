namespace RestaurantManagement.Domain.Entities;

public class BookingLog
{
    public Ulid BookingLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogDetails { get; set; }
    public DateTime LogDate { get; set; }
    public User? User { get; set; }
}
