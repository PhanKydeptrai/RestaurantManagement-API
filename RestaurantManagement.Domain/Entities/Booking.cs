namespace RestaurantManagement.Domain.Entities;

public class Booking
{
    public Ulid BookId { get; set; }
    public DateTime Time { get; set; }
    public decimal BookingPrice { get; set; }
    public string PaymentStatus { get; set; }
    public Ulid CustomerId { get; set; }
    public string? Note { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }
    public ICollection<BookingChangeLog>? BookingChangeLogs { get; set; }
    public Bill? Bill { get; set; }

}
