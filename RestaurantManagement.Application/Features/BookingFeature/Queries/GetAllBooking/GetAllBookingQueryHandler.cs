using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BookingDtos;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BookingFeature.Queries.GetAllBooking;

public class GetAllBookingQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAllBookingQuery, PagedList<BookingResponse>>
{
    public async Task<Result<PagedList<BookingResponse>>> Handle(GetAllBookingQuery request, CancellationToken cancellationToken)
    {
        var bookingQuery = context.Bookings
            .Include(a => a.BookingDetails)
            .Include(a => a.Customer)
            .ThenInclude(a => a.User).AsQueryable();

        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            bookingQuery = bookingQuery.Where(x => x.Customer.User.Phone.Contains(request.searchTerm));
        }

        //Filter
        if (!string.IsNullOrEmpty(request.filterPaymentStatus)) //filter theo payment status
        {
            bookingQuery = bookingQuery.Where(x => x.PaymentStatus == request.filterPaymentStatus);
        }

        if (!string.IsNullOrEmpty(request.filterBookingStatus)) //filter theo booking status
        {
            bookingQuery = bookingQuery.Where(x => x.BookingStatus == request.filterBookingStatus);
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
        var bookings = bookingQuery
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
                a.Note)).AsQueryable();
        var bookingsList = await PagedList<BookingResponse>.CreateAsync(bookings, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<BookingResponse>>.Success(bookingsList);
    }
}
