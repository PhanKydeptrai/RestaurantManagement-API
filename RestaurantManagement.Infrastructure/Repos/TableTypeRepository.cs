using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class TableTypeRepository : ITableTypeRepository
{
    private readonly RestaurantManagementDbContext _context;

    public TableTypeRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsTableTypeNameUnique(string tableTypeName)
    {
        return await _context.TableTypes.AnyAsync(x => x.TableTypeName == tableTypeName);

    }
}
