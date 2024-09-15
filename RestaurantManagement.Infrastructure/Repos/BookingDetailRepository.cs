using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class BookingDetailRepository : IBookingDetailRepository
{
    private readonly RestaurantManagementDbContext _context;
    public BookingDetailRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task AddBookingDetail(BookingDetail bookingDetail)
    {
        await _context.BookingDetails.AddAsync(bookingDetail);
    }

    public void DeleteBookingDetail(BookingDetail bookingDetail)
    {
        _context.BookingDetails.Remove(bookingDetail);
    }

    public async Task<IEnumerable<BookingDetail>> GetAllBookingDetails()
    {
        return await _context.BookingDetails.ToListAsync();
    }

    public async Task<BookingDetail?> GetBookingDetailById(Guid id)
    {
        return await _context.BookingDetails.FindAsync(id);
    }

    public async Task<IEnumerable<BookingDetail>> GetBookingDetailsByBookingId(Guid id)
    {
        return await _context.BookingDetails.Where(x => x.BookId == id).ToListAsync();
    }

    public IQueryable<BookingDetail> GetQueryableBookingDetails()
    {
        return _context.BookingDetails.AsQueryable();
    }

    public void UpdateBookingDetail(BookingDetail bookingDetail)
    {
        _context.BookingDetails.Update(bookingDetail);
    }
}
