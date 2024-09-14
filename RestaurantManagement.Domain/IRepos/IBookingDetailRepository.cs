using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IBookingDetailRepository
{
    //CRUD
    Task<IEnumerable<BookingDetail>> GetAllBookingDetails();
    Task<BookingDetail> GetBookingDetailById(int id);
    Task AddBookingDetail(BookingDetail bookingDetail);
    void UpdateBookingDetail(BookingDetail bookingDetail);
    void DeleteBookingDetail(BookingDetail bookingDetail);
    //Queries
    IQueryable<BookingDetail> GetQueryableBookingDetails();
    Task<IEnumerable<BookingDetail>> GetBookingDetailsByBookingId(int id);
}
