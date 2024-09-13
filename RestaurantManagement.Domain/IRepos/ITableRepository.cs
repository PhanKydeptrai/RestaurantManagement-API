using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableRepository
{
    //CRUD
    Task<IEnumerable<Table>> GetAllTables();
    Task<Table?> GetTableById(Guid id);
    Task AddTable(Table table);
    void UpdateTable(Table table);
    void DeleteTable(Table table);
    
    //Queries
    Task<string?> GetTableStatus(Guid id); //Get table status 
    IQueryable<Table> GetQueryableOfTable(); //Get IQueryable of Table
    Task<bool> IsTableNameAvailable(string tableName); //Check if table name is available
    
}
