namespace RestaurantManagement.Domain.Entities;

public class BookingChangeLog
{
    public Ulid BookingChangeLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogMessage { get; set; }
    public string Note { get; set; }
    public DateTime LogDate { get; set; }
    public Ulid BookId { get; set; }
    public User User { get; set; }
    public Booking? Booking { get; set; }
    
}
