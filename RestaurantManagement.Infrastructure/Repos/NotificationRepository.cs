using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class NotificationRepository : INotificationRepository
{

    private readonly RestaurantManagementDbContext _context;
    public NotificationRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task CreateNotification(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public void DeleteNotification(Notification notification)
    {
        _context.Notifications.Remove(notification);
    }

    public async Task<IEnumerable<Notification>> GetAllNotifications()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<Notification?> GetNotificationById(Ulid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUserId(Ulid id)
    {
        return await _context.Notifications.Where(x => x.UserId == id).ToListAsync();
    }

    public IQueryable<Notification> GetQueryableNotifications()
    {
        return _context.Notifications.AsQueryable();
    }

    public void UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
    }
}
