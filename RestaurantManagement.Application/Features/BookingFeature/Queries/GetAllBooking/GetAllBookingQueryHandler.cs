using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetAllBooking;

public class GetAllBookingQueryHandler : IQueryHandler<GetAllBookingQuery, PagedList<BookingResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllBookingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<BookingResponse>>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
    {
        var bookingQuery = _context.Bookings
            .Include(a => a.Customer)
            .ThenInclude(a => a.User).AsQueryable();

        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            bookingQuery = bookingQuery.Where(x => x.Customer.User.Phone.Contains(request.searchTerm));
        }

        //Filter
        if (!string.IsNullOrEmpty(request.filterPaymentStatus))
        {
            bookingQuery = bookingQuery.Where(x => x.PaymentStatus == request.filterPaymentStatus);
        }

        //Sort
        Expression<Func<Booking, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "bookid" => x => x.BookId,
            _ => x => x.BookId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            bookingQuery = bookingQuery.OrderByDescending(keySelector);
        }
        else
        {
            bookingQuery = bookingQuery.OrderBy(keySelector);
        }

        //paged
        var categories = bookingQuery
            .Select(a => new BookingResponse(
                a.BookId,
                a.Customer.UserId,
                a.Customer.User.FirstName,
                a.Customer.User.LastName,
                a.Customer.User.Email,
                a.Customer.User.Phone,
                a.BookingDate,
                a.BookingTime,
                a.BookingPrice,
                a.PaymentStatus,
                a.NumberOfCustomers,
                a.Note)).AsQueryable();
        var bookingsList = await PagedList<BookingResponse>.CreateAsync(categories, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<BookingResponse>>.Success(bookingsList);
    }
}
