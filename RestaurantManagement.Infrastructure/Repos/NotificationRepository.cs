using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class NotificationRepository(RestaurantManagementDbContext context) : INotificationRepository
{
    public async Task CreateNotification(Notification notification)
    {
        await context.Notifications.AddAsync(notification);
    }

    public void DeleteNotification(Notification notification)
    {
        context.Notifications.Remove(notification);
    }

    public async Task<IEnumerable<Notification>> GetAllNotifications()
    {
        return await context.Notifications.ToListAsync();
    }

    public async Task<Notification?> GetNotificationById(Ulid id)
    {
        return await context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserId(Ulid id)
    {
        return await context.Notifications.Where(x => x.UserId == id).ToListAsync();
    }

    public IQueryable<Notification> GetQueryableNotifications()
    {
        return context.Notifications.AsQueryable();
    }

    public void UpdateNotification(Notification notification)
    {
        context.Notifications.Update(notification);
    }
}
