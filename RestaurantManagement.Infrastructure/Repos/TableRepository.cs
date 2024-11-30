using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class TableRepository(RestaurantManagementDbContext context) : ITableRepository
{
    public async Task AddTable(Table table)
    {
        await context.Tables.AddAsync(table);
    }

    public void DeleteTable(Table table)
    {
        context.Tables.Remove(table);
    }

    public async Task<string?> GetActiveStatus(int id)
    {
        return await context.Tables.Where(t => t.TableId == id)
            .Select(t => t.ActiveStatus)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Table>> GetAllTables()
    {
        return await context.Tables.ToListAsync();
    }

    public IQueryable<Table> GetQueryableOfTable()
    {
        IQueryable<Table> tables = context.Tables;
        return tables;
    }


    public async Task<Table?> GetTableById(Ulid id)
    {
        return await context.Tables.FindAsync(id);
    }

    public async Task<bool> IsTableOccupied(int id)
    {
        return await context.Tables.AsNoTracking()
            .AnyAsync(t => t.TableId == id && t.ActiveStatus == "Occupied");
    }

    public async Task<bool> IsTableHasUnpaidOrder(int id)
    {
        return await context.Tables
            .AsNoTracking()
            .Include(a => a.Orders)
            .Where(a => a.TableId == id)
            .Select(a => a.Orders.Any(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsTableHasBooking(int id)
    {
        return await context.Tables
            .Include(a => a.BookingDetails)
            .ThenInclude(a => a.Booking)
            .Select(a => a.BookingDetails.Any(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();
            
    }

    public async Task<string?> GetTableStatus(int id)
    {
        return await context.Tables.AsNoTracking()
                                    .Where(t => t.TableId == id)
                                    .Select(t => t.TableStatus)
                                    .FirstOrDefaultAsync();
    }

    public async Task<bool> IsTableExistAndActive(int id)
    {
        return await context.Tables
            .AsNoTracking()
            .AnyAsync(t => t.TableId == id && t.TableStatus == "Active");
    }


    public async Task<bool> IsTableJustExist(int id)
    {
        return await context.Tables.AsNoTracking()
            .AnyAsync(t => t.TableId == id);
    }

    //Cập nhật trạng thái bàn khi booking
    public async Task UpdateActiveStatus(int id, string status)
    {
        await context.Tables
            .Where(a => a.TableId == id)
            .ExecuteUpdateAsync(
                a => a.SetProperty(a => a.ActiveStatus, status));
    }

    public async Task RestoreTable(int id) //table status
    {
        await context.Tables.Where(a => a.TableId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.TableStatus, "Active"));
        //
    }

    public async Task DeleteTable(int id) //table status
    {
        await context.Tables.Where(a => a.TableId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.TableStatus, "InActive"));
        //
    }

    public async Task<bool> IsTableAvailable(int id)
    {
        return await context.Tables.AsNoTracking()
            .AnyAsync(a => a.ActiveStatus == "Empty");
    }

    public async Task<int> GetTableCapacity(int id)
    {
        return await context.Tables
                    .Where(a => a.TableId == id)
                    .AsNoTracking()
                    .Include(a => a.TableType)
                    .Select(a => a.TableType.TableCapacity)
                    .FirstOrDefaultAsync();
    }

    public async Task<Ulid?> GetCustomerIdByTableId(int tableId)
    {
        //
        BookingDetail? bookingDetail = await context.Tables
            .Include(a => a.BookingDetails)
            .ThenInclude(a => a.Booking)
            .Where(a => a.TableId == tableId)
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();
            
        if (bookingDetail == null)
        {
            return null;
        }
        
        Ulid? customerId = await context.Bookings.Where(a => a.BookId == bookingDetail.BookId)
            .Select(a => a.CustomerId)
            .FirstOrDefaultAsync();

        return customerId;
    }
}
