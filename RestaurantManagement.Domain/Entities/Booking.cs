namespace RestaurantManagement.Domain.Entities;

public class Booking
{
    public Guid BookingId { get; set; }
    public DateTime Time { get; set; }
    public decimal BookingPrice { get; set; }
    public string Status { get; set; }
    public Customer? Customer { get; set; }
    public Guid CustomerId { get; set; }
    public string Note { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }

}
