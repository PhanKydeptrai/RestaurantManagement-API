namespace RestaurantManagement.Domain.IRepos;

public interface ITableTypeRepository
{
    Task<bool> IsTableTypeNameUnique(string tableTypeName);
}
