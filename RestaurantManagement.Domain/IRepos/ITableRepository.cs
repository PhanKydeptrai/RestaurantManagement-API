using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableRepository
{
    //Get list of table
    Task<IEnumerable<Table>> GetAllTables();
    //Get table by id
    Task<Table?> GetTableById(Guid id);
    //Add table 
    Task AddTable(Table table);
    //Update table
    void UpdateTable(Table table);
    //Delete table
    void DeleteTable(Table table);
    //Get table status 
    Task<string?> GetTableStatus(Guid id);
}
