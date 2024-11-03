using RestaurantManagement.Domain.DTOs.BookingDtos;
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
    Task<bool> IsBookingDateValid(DateOnly bookingDate);
    Task<bool> IsBookingTimeValid(TimeOnly bookingTime);
    Task<bool> IsCapacityAvailable(int numberOfCustomers);
    Task<bool> IsBookingStatusValid(Ulid id);
    Task<BookingResponse?> GetBookingResponseById(Ulid id);
    Task<BookingResponse[]> GetBookingResponseByUserId(Ulid id);
    Task UpdateBookingStatus(Ulid id);
    Task<int> GetNumberOfCustomers(Ulid id);
    //Queries
    IQueryable<Booking> GetQueryableBookings();
    Task<IEnumerable<Booking>> GetBookingsByCustomerId(Ulid id);
}
