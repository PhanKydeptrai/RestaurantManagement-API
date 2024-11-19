namespace RestaurantManagement.Domain.Entities;

public class Booking
{
    public Ulid BookId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly BookingTime { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal BookingPrice { get; set; }
    public string PaymentStatus { get; set; }
    public string BookingStatus { get; set; }
    public int NumberOfCustomers { get; set; }
    public Ulid CustomerId { get; set; }
    public string? Note { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }
    public Bill? Bill { get; set; }
}
