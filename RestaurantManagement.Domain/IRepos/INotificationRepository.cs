using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface INotificationRepository
{
    //CRUD
    Task CreateNotification(Notification notification);
    Task<IEnumerable<Notification>> GetAllNotifications();
    Task<Notification?> GetNotificationById(Guid id);
    void UpdateNotification(Notification notification);
    void DeleteNotification(Notification notification);
    //Queries
    IQueryable<Notification> GetQueryableNotifications();
    Task<IEnumerable<Notification>> GetNotificationsByUserId(Guid id);
    
    

}
