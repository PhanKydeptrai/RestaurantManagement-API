using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableRepository
{
    //CRUD
    Task<IEnumerable<Table>> GetAllTables();
    Task<Table?> GetTableById(Ulid id);
    Task AddTable(Table table);
    void UpdateTable(Table table);
    Task UpdateTableStatus(Ulid id, string status);
    Task UpdateActiveStatus(Ulid id, string status);
    void DeleteTable(Table table);
    Task<bool> IsTableExist(Ulid id);
    //Queries
    Task<string?> GetTableStatus(Ulid id); //Get table status 
    IQueryable<Table> GetQueryableOfTable(); //Get IQueryable of Table
    Task<string?> GetActiveStatus(Ulid id);
    
}
