namespace RestaurantManagement.Domain.Entities;

public class TableTypeLog
{
    public Ulid TableTypeLogId { get; set; }
    public Ulid UserId { get; set; }
    public string LogDetails { get; set; }
    public DateTime LogDate { get; set; }
    public User? User { get; set; }
}
