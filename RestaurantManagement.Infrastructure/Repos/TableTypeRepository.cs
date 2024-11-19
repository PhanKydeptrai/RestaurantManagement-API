using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.TableTypeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class TableTypeRepository(RestaurantManagementDbContext context) : ITableTypeRepository
{
    public async Task DeleteTableType(Ulid tableTypeId)
    {
        await context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.Status, "InActive"));

        await context.Tables.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.ActiveStatus, "InActive"));
    }
    public async Task RestoreTableType(Ulid tableTypeId)
    {
        await context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.Status, "Active"));

        await context.Tables.Where(a => a.TableTypeId == tableTypeId)
            .ExecuteUpdateAsync(a => a.SetProperty(x => x.ActiveStatus, "Active"));
    }
    public async Task<TableTypeResponse?> GetTableTypeById(Ulid tableTypeId)
    {
        return await context.TableTypes.Select(a => new TableTypeResponse(
            a.TableTypeId,
            a.TableTypeName,
            a.Status,
            a.TableCapacity,
            a.ImageUrl,
            a.TablePrice,
            a.Description)).FirstOrDefaultAsync();
    }

    public Task<string> GetTableTypeStatus(Ulid tableTypeId)
    {
        return context.TableTypes.Where(a => a.TableTypeId == tableTypeId)
            .Select(a => a.Status).FirstOrDefaultAsync();
    }

    public Task<bool> IsTableTypeExist(Ulid tableTypeId)
    {
        return context.TableTypes.AsNoTracking().AnyAsync(x => x.TableTypeId == tableTypeId);
    }

    public async Task<bool> IsTableTypeNameUnique(string tableTypeName)
    {
        return await context.TableTypes.AnyAsync(x => x.TableTypeName == tableTypeName);

    }

    public async Task<bool> IsTableTypeNameUnique(string tableTypeName, Ulid tableTypeId)
    {
        return await context.TableTypes.AnyAsync(x => x.TableTypeName == tableTypeName && x.TableTypeId != tableTypeId);
    }

    public async Task<List<TableTypeInfo>> GetAllTableTypeInfo()
    {
        return await context.TableTypes.Where(a => a.Status == "Active")
            .Select(a => new TableTypeInfo(
                a.TableTypeId, 
                a.TableTypeName))
                .ToListAsync();
    }

}
