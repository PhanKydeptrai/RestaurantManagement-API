using System.IO.Pipes;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly RestaurantManagementDbContext _context;
    public UnitOfWork(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
