namespace RestaurantManagement.Domain.Entities;

public class Order
{
    public Guid OrderId { get; set; }
    public string OrderStatus { get; set; }
    public decimal Total { get; set; }
    public DateTime OrderTime { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Liên kết với OrderDetail
    public ICollection<OrderDetail>? OrderDetails { get; set; }
}
