namespace RestaurantManagement.Domain.Entities;

public class BookingDetail
{
    public Ulid BookingDetailId { get; set; }
    public Ulid TableId { get; set; }
    public string Status { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Ulid BookId { get; set; }
    public Table? Table { get; set; }   
    public Booking? Booking { get; set; }

}
