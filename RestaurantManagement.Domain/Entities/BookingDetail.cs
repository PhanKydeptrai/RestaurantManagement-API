namespace RestaurantManagement.Domain.Entities;

public class BookingDetail
{
    public Guid BookingDetailId { get; set; }
    public Guid BookId { get; set; }
    public string Status { get; set; }
    public Guid TableId { get; set; }
    public Table? Table { get; set; }
    public Booking? Booking { get; set; }

}
