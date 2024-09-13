using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class TableRepository : ITableRepository
{
    private readonly RestaurantManagementDbContext _context;
    public TableRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddTable(Table table)
    {
        await _context.Tables.AddAsync(table);
    }

    public void DeleteTable(Table table)
    {
        _context.Tables.Remove(table);
    }

    public async Task<IEnumerable<Table>> GetAllTables()
    {
        return await _context.Tables.ToListAsync();
    }

    public async Task<Table?> GetTableById(Guid id)
    {
        return await _context.Tables.FirstOrDefaultAsync(t => t.TableId == id);
    }

    public async Task<string?> GetTableStatus(Guid id)
    {
        return await _context.Tables.Where(t => t.TableId == id)
                                    .Select(t => t.TableStatus)
                                    .FirstOrDefaultAsync();
    }

    public void UpdateTable(Table table)
    {
        _context.Tables.Update(table);  
    }
    

}
