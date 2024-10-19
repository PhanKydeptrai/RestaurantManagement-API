namespace RestaurantManagement.Domain.Entities;

public class OrderChangeLog
{
    public Ulid OrderChangeLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogMessage { get; set; }
    public string Note { get; set; }
    public DateTime LogDate { get; set; }
    public Ulid OrderId { get; set; }
    public User? User { get; set; }
    public Order? Order { get; set; }
}
