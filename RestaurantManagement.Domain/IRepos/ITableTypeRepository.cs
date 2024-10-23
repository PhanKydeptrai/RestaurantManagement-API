using RestaurantManagement.Domain.DTOs.TableTypeDto;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableTypeRepository
{
    Task<bool> IsTableTypeNameUnique(string tableTypeName);
    Task<TableTypeResponse?> GetTableTypeById(Ulid tableTypeId);
    Task<bool> IsTableTypeExist(Ulid tableTypeId);
    Task<bool> IsTableTypeNameUnique(string tableTypeName, Ulid tableTypeId);
    Task DeleteTableType(Ulid tableTypeId);
    Task RestoreTableType(Ulid tableTypeId);
    Task<string> GetTableTypeStatus(Ulid tableTypeId);
}
