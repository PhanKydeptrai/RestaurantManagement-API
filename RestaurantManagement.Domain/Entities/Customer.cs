namespace RestaurantManagement.Domain.Entities;

public class Customer
{
    public Ulid CustomerId { get; set; }
    public Ulid UserId { get; set; }
    public string CustomerStatus { get; set; }
    public string CustomerType { get; set; }
    public User? User { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<CustomerVoucher>? CustomerVouchers { get; set; }

}
