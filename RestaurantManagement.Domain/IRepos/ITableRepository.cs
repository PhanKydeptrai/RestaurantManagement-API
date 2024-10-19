using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableRepository
{
    //CRUD
    Task<IEnumerable<Table>> GetAllTables();
    Task<Table?> GetTableById(Ulid id);
    Task AddTable(Table table);
    void UpdateTable(Table table);
    void DeleteTable(Table table);

    //Queries
    Task<string?> GetTableStatus(Ulid id); //Get table status 
    IQueryable<Table> GetQueryableOfTable(); //Get IQueryable of Table


}
