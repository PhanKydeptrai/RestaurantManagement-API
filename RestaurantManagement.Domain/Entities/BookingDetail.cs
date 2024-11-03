namespace RestaurantManagement.Domain.Entities;

public class BookingDetail
{
    public Ulid BookingDetailId { get; set; }
    public int TableId { get; set; }
    public Ulid BookId { get; set; }
    public Table? Table { get; set; }
    public Booking? Booking { get; set; }
    
}
