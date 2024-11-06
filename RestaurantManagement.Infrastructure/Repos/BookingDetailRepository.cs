using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class BookingDetailRepository(RestaurantManagementDbContext context) : IBookingDetailRepository
{
    public async Task AddBookingDetail(BookingDetail bookingDetail)
    {
        await context.BookingDetails.AddAsync(bookingDetail);
    }

    public void DeleteBookingDetail(BookingDetail bookingDetail)
    {
        context.BookingDetails.Remove(bookingDetail);
    }

    public async Task<IEnumerable<BookingDetail>> GetAllBookingDetails()
    {
        return await context.BookingDetails.ToListAsync();
    }

    public async Task<BookingDetail?> GetBookingDetailById(Ulid id)
    {
        return await context.BookingDetails.FindAsync(id);
    }

    public async Task<IEnumerable<BookingDetail>> GetBookingDetailsByBookingId(Ulid id)
    {
        return await context.BookingDetails.Where(x => x.BookId == id).ToListAsync();
    }

    public IQueryable<BookingDetail> GetQueryableBookingDetails()
    {
        return context.BookingDetails.AsQueryable();
    }

    public void UpdateBookingDetail(BookingDetail bookingDetail)
    {
        context.BookingDetails.Update(bookingDetail);
    }
}
