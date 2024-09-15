using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class SystemLogRepository : ISystemLogRepository
{
    private readonly RestaurantManagementDbContext _context;
    public SystemLogRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task CreateSystemLog(SystemLog systemLog)
    {
        await _context.SystemLogs.AddAsync(systemLog);
    }

    public void DeleteSystemLog(SystemLog systemLog)
    {
        _context.SystemLogs.Remove(systemLog);
    }

    public async Task<IEnumerable<SystemLog>> GetAllSystemLogs()
    {
        return await _context.SystemLogs.ToListAsync();
    }

    public IQueryable<SystemLog> GetQueryableSystemLogs()
    {
        return _context.SystemLogs.AsQueryable();
    }

    public async Task<SystemLog?> GetSystemLogById(Guid id)
    {
        return await _context.SystemLogs.FindAsync(id);
    }

    public async Task<IEnumerable<SystemLog>> GetSystemLogsByUserId(Guid id)
    {
        return await _context.SystemLogs.Where(i => i.UserId == id).ToListAsync();
    }

    public void UpdateSystemLog(SystemLog systemLog)
    {
        _context.SystemLogs.Update(systemLog);
    }
}
