namespace RestaurantManagement.Domain.Entities;

public class Customer
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string CustomerStatus { get; set; }
    public string CustomerType { get; set; }
    public User? User { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
