using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ISystemLogRepository
{
    //CRUD
    Task CreateSystemLog(SystemLog systemLog);
    Task<IEnumerable<SystemLog>> GetAllSystemLogs();
    Task<SystemLog?> GetSystemLogById(Guid id);
    void UpdateSystemLog(SystemLog systemLog);
    void DeleteSystemLog(SystemLog systemLog);
    //Queries
    IQueryable<SystemLog> GetQueryableSystemLogs();
    Task<IEnumerable<SystemLog>> GetSystemLogsByUserId(Guid id);
}
