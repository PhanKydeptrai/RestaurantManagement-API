namespace RestaurantManagement.Domain.Entities;

public class Table
{
    public Ulid TableId { get; set; }
    public Ulid TableTypeId { get; set; }
    public string TableStatus { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public TableType? TableType { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }
}
