namespace RestaurantManagement.Domain.Entities;

public class SystemLog
{
    public Ulid SystemLogId { get; set; }
    public string LogDetail { get; set; }
    public Ulid UserId { get; set; }
    public DateTime LogDate { get; set; }
    public User? User { get; set; }
}
