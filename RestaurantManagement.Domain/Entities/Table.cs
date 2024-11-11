namespace RestaurantManagement.Domain.Entities;

public class Table
{ 
    public int TableId { get; set; }
    public Ulid TableTypeId { get; set; }
    public string TableStatus { get; set; }
    public string ActiveStatus { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public TableType? TableType { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }
}
