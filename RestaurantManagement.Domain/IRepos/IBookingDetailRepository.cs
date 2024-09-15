using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IBookingDetailRepository
{
    //CRUD
    Task<IEnumerable<BookingDetail>> GetAllBookingDetails();
    Task<BookingDetail?> GetBookingDetailById(Guid id);
    Task AddBookingDetail(BookingDetail bookingDetail);
    void UpdateBookingDetail(BookingDetail bookingDetail);
    void DeleteBookingDetail(BookingDetail bookingDetail);
    //Queries
    IQueryable<BookingDetail> GetQueryableBookingDetails();
    Task<IEnumerable<BookingDetail>> GetBookingDetailsByBookingId(Guid id);
}
