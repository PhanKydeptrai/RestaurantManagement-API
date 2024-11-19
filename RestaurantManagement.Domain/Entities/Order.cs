namespace RestaurantManagement.Domain.Entities;

public class Order
{
    public Ulid OrderId { get; set; }
    public string PaymentStatus { get; set; }
    public decimal Total { get; set; }
    public Ulid? CustomerId { get; set; }
    // public Ulid TableId { get; set; }
    public int TableId { get; set; }
    public DateTime OrderTime { get; set; }
    public string? Note { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderDetail>? OrderDetails { get; set; }
    public Table? Table { get; set; }
    public Bill? Bill { get; set; }
}
