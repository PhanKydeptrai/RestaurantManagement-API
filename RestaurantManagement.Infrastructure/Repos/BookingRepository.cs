using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class BookingRepository(RestaurantManagementDbContext context) : IBookingRepository
{
    public async Task AddBooking(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
    }

    public void DeleteBooking(Booking booking)
    {
        context.Bookings.Remove(booking);
    }

    public async Task<IEnumerable<Booking>> GetAllBookings()
    {
        return await context.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetBookingById(Ulid id)
    {
        return await context.Bookings.FindAsync(id);
    }

    public async Task<bool> IsBookingCanceled(Ulid id)
    {
        return await context.Bookings.AnyAsync(a => a.BookingStatus == "Canceled" && a.BookId == id);
    }

    public async Task<BookingResponse?> GetBookingResponseById(Ulid id)
    {
        return await context.Bookings
            .AsNoTracking()
            .Include(a => a.Customer)
            .ThenInclude(a => a.User)
            .Where(a => a.BookId == id)
            .Select(a => new BookingResponse(
                a.BookId,
                a.Customer.UserId,
                a.Customer.User.FirstName,
                a.Customer.User.LastName,
                a.Customer.User.Email,
                a.Customer.User.Phone,
                a.BookingDetails.FirstOrDefault().TableId,
                a.BookingDate,
                a.BookingTime,
                a.BookingPrice,
                a.PaymentStatus,
                a.BookingStatus,
                a.NumberOfCustomers,
                a.Note
            )).FirstOrDefaultAsync();
    }

    public async Task<BookingResponse[]> GetBookingResponseByUserId(Ulid id)
    {
        return await context.Bookings
            .AsNoTracking()
            .Include(a => a.Customer)
            .ThenInclude(a => a.User)
            .Where(a => a.Customer.UserId == id)
            .Select(a => new BookingResponse(
                a.BookId,
                a.Customer.UserId,
                a.Customer.User.FirstName,
                a.Customer.User.LastName,
                a.Customer.User.Email,
                a.Customer.User.Phone,
                a.BookingDetails.FirstOrDefault().TableId,
                a.BookingDate,
                a.BookingTime,
                a.BookingPrice,
                a.PaymentStatus,
                a.BookingStatus,
                a.NumberOfCustomers,
                a.Note
            )).ToArrayAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCustomerId(Ulid id)
    {
        return await context.Bookings.Where(i => i.CustomerId == id).ToListAsync();
    }

    public async Task<int> GetNumberOfCustomers(Ulid id)
    {
        return await context.Bookings
                    .AsNoTracking()
                    .Where(a => a.BookId == id)
                    .Select(a => a.NumberOfCustomers)
                    .FirstOrDefaultAsync();
    }

    public IQueryable<Booking> GetQueryableBookings()
    {
        return context.Bookings.AsQueryable();
    }

    public async Task<bool> IsBookingDateValid(DateOnly bookingDate)
    {
        //NOTE: Unstrusted data
        #region UnStable code
            // string dateString = bookingDate.ToString();
            // bool dateFormatOk = DateOnly.TryParseExact(
            //     dateString, 
            //     "MM/dd/yyyy",
            //     CultureInfo.InvariantCulture,
            //     DateTimeStyles.None,
            //     out _
            // );
    
            // if (dateFormatOk && bookingDate < DateOnly.FromDateTime(DateTime.Now))
            // {
            //     return false;
            // }
            // return true;
        #endregion

        if (bookingDate < DateOnly.FromDateTime(DateTime.Now))
        {
            return false;
        }
        return true;
    }

    public async Task<bool> IsBookingStatusValid(Ulid id)
    {
        return await context.Bookings.AnyAsync(a => a.BookingStatus == "Waiting" && a.BookId == id && a.PaymentStatus == "Paid");
    }

    public async Task<bool> IsBookingTimeValid(TimeOnly bookingTime)
    {
        TimeOnly startTime = new TimeOnly(8, 0);
        TimeOnly endTime = new TimeOnly(20, 0);

        if (bookingTime >= startTime && bookingTime <= endTime)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> IsCapacityAvailable(int numberOfCustomers)
    {

        // var tableQuery = _context.Tables
        //     .Where(a => a.TableStatus == "Active" && a.ActiveStatus == "Empty")
        //     .AsQueryable();

        // List<TableType> taleTypes = await _context.TableTypes
        //     .Where(a => a.Status == "Active")
        //     .Select(a => new TableType
        //     { 
        //         TableTypeId = a.TableTypeId, 
        //         TableCapacity = a.TableCapacity 
        //     }).ToListAsync();
        // int countSeat = 0;
        // foreach (var item in taleTypes)
        // {
        //     int tableCount = await tableQuery.CountAsync(a => a.TableTypeId == item.TableTypeId);
        //     countSeat = countSeat + (tableCount * item.TableCapacity);
        // }

        // if (countSeat <= numberOfCustomers)
        // {
        //     return false;
        // }
        // return true;
        List<TableType> tableTypes = await context.TableTypes
        .Where(a => a.Status == "Active")
        .Select(a => new TableType
        {
            TableTypeId = a.TableTypeId,
            TableCapacity = a.TableCapacity
        }).ToListAsync();

        var tableQuery = context.Tables.AsQueryable()
            .Where(a => a.TableStatus == "Active" && a.ActiveStatus == "Empty");


        int countSeat = 0;
        foreach (var item in tableTypes)
        {
            int tableCount = await tableQuery.CountAsync(a => a.TableTypeId == item.TableTypeId);
            countSeat = countSeat + (tableCount * item.TableCapacity);
        }

        if (countSeat <= numberOfCustomers)
        {
            return false;
        }
        return true;
    }

    public void UpdateBooking(Booking booking)
    {
        context.Bookings.Update(booking);
    }

    public async Task UpdateBookingStatus(Ulid id)
    {
        await context.Bookings.Where(a => a.BookId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.BookingStatus, "Seated"));
    }

    public async Task CancelBooking(Ulid id)
    {
        await context.Bookings.Where(a => a.BookId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.BookingStatus, "Canceled"));
    }

    public async Task<bool> IsBookingExist(Ulid id)
    {
        return await context.Bookings.AsNoTracking().AnyAsync(a => a.BookId == id);
    }

    public async Task<bool> IsBookingCompleted(Ulid id)
    {
        return await context.Bookings.AsNoTracking().AnyAsync(a => a.BookingStatus == "Completed" && a.BookId == id);
    }
}
