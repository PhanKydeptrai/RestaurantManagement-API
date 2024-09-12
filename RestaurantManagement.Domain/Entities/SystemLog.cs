namespace RestaurantManagement.Domain.Entities;

public class SystemLog
{
    public Guid LogId { get; set; }
    public string LogDetail { get; set; }
    public DateTime  LogDate { get; set; }
    public string UserId { get; set; }
}
