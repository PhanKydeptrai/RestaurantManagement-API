using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.Entities;
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

    public async Task DeleteTableType(Ulid tableTypeId)
    {
        await _context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.Status, "nhd"));
    }
    public async Task RestoreTableType(Ulid tableTypeId)
    {
        await _context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.Status, "hd"));
    }
    public async Task<TableTypeResponse?> GetTableTypeById(Ulid tableTypeId)
    {
        return await _context.TableTypes.Select(a => new TableTypeResponse(
            a.TableTypeId,
            a.TableTypeName,
            a.Status,
            a.ImageUrl,
            a.TablePrice,
            a.Description)).FirstOrDefaultAsync();
    }

    public Task<string> GetTableTypeStatus(Ulid tableTypeId)
    {
        return _context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .Select(a => a.Status).FirstOrDefaultAsync();
    }

    public Task<bool> IsTableTypeExist(Ulid tableTypeId)
    {
        return _context.TableTypes.AnyAsync(x => x.TableTypeId == tableTypeId);
    }

    public async Task<bool> IsTableTypeNameUnique(string tableTypeName)
    {
        return await _context.TableTypes.AnyAsync(x => x.TableTypeName == tableTypeName);

    }

    public async Task<bool> IsTableTypeNameUnique(string tableTypeName, Ulid tableTypeId)
    {
        return await _context.TableTypes.AnyAsync(x => x.TableTypeName == tableTypeName && x.TableTypeId != tableTypeId);
    }

    public async Task<List<TableTypeInfo>> GetAllTableTypeInfo()
    {
        return await _context.TableTypes.Where(a => a.Status == "hd")
            .Select(a => new TableTypeInfo(
                a.TableTypeId, 
                a.TableTypeName))
                .ToListAsync();
    }

}
