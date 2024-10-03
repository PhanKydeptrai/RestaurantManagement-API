using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class BookingRepository : IBookingRepository
{
    private readonly RestaurantManagementDbContext _context;
    public BookingRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task AddBooking(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public void DeleteBooking(Booking booking)
    {
        _context.Bookings.Remove(booking);
    }

    public async Task<IEnumerable<Booking>> GetAllBookings()
    {
        return await _context.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetBookingById(Ulid id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByCustomerId(Ulid id)
    {
        return await _context.Bookings.Where(i => i.CustomerId == id).ToListAsync();
    }

    public IQueryable<Booking> GetQueryableBookings()
    {
        return _context.Bookings.AsQueryable();
    }

    public void UpdateBooking(Booking booking)
    {
        _context.Bookings.Update(booking);
    }
}
