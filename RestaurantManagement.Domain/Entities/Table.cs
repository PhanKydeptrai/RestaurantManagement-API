namespace RestaurantManagement.Domain.Entities;

public class Table
{
    public Guid TableId { get; set; }
    public Guid BookingDetailId { get; set; }
    public string TableName { get; set; }
    public ICollection<BookingDetail>? BookingDetails { get; set; }
    public string TableType { get; set; }
    public string TableStatus { get; set; }
    public string Desciption { get; set; }
}
