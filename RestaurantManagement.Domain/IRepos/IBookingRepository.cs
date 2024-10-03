using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IBookingRepository
{
    //CRUD
    Task<IEnumerable<Booking>> GetAllBookings();
    Task<Booking?> GetBookingById(Ulid id);
    Task AddBooking(Booking booking);
    void UpdateBooking(Booking booking);
    void DeleteBooking(Booking booking);

    //Queries
    IQueryable<Booking> GetQueryableBookings();
    Task<IEnumerable<Booking>> GetBookingsByCustomerId(Ulid id); 
}
