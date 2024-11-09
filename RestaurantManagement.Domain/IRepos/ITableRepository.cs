using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ITableRepository
{
    //CRUD
    Task<IEnumerable<Table>> GetAllTables();
    Task<bool> IsTableOccupied(int id);
    Task<Table?> GetTableById(Ulid id);
    Task AddTable(Table table);
    Task<bool> IsTableHasUnpaidOrder(int id);
    Task<bool> IsTableHasBooking(int id);
    Task<Ulid?> GetCustomerIdByTableId(int tableId); //booked
    
    Task RestoreTable(int id);
    Task DeleteTable(int id);
    Task<bool> IsTableAvailable(int id);
    Task UpdateActiveStatus(int id, string status);
    void DeleteTable(Table table);
    Task<int> GetTableCapacity(int id);
    Task<bool> IsTableExist(int id);
    //Queries
    Task<string?> GetTableStatus(int id); //Get table status 
    IQueryable<Table> GetQueryableOfTable(); //Get IQueryable of Table
    Task<string?> GetActiveStatus(int id);
    
}
