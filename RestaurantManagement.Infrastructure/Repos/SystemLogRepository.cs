using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class SystemLogRepository(RestaurantManagementDbContext context) : ISystemLogRepository
{
    public async Task CreateSystemLog(SystemLog systemLog)
    {
        await context.SystemLogs.AddAsync(systemLog);
    }

    public void DeleteSystemLog(SystemLog systemLog)
    {
        context.SystemLogs.Remove(systemLog);
    }

    public async Task<IEnumerable<SystemLog>> GetAllSystemLogs()
    {
        return await context.SystemLogs.ToListAsync();
    }

    public IQueryable<SystemLog> GetQueryableSystemLogs()
    {
        return context.SystemLogs.AsQueryable();
    }

    public async Task<SystemLog?> GetSystemLogById(Ulid id)
    {
        return await context.SystemLogs.FindAsync(id);
    }

    public async Task<IEnumerable<SystemLog>> GetSystemLogsByUserId(Ulid id)
    {
        return await context.SystemLogs.Where(i => i.UserId == id).ToListAsync();
    }

    public void UpdateSystemLog(SystemLog systemLog)
    {
        context.SystemLogs.Update(systemLog);
    }
}
