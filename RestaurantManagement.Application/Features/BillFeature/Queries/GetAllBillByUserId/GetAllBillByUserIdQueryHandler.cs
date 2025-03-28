using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BillDtos;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBillByUserId;

public class GetAllBillByUserIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAllBillByUserIdQuery, PagedList<BillResponse>>
{
    public async Task<Result<PagedList<BillResponse>>> Handle(GetAllBillByUserIdQuery request, CancellationToken cancellationToken)
    {
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);


        var billQuery = context.Bills
            .AsNoTracking()
            .Where(a => a.Booking.Customer.UserId == Ulid.Parse(userId))
            .Include(a => a.Booking)
            .ThenInclude(a => a.Customer.User)
            .Include(a => a.Order)
            .ThenInclude(a => a.OrderDetails)
            .ThenInclude(a => a.Meal)
            .AsQueryable();


        //Search
        // if (!string.IsNullOrEmpty(request.searchTerm))
        // {
        //     billQuery = billQuery.Where(x => x. .Contains(request.searchTerm));
        // }

        //Filter
        // if (!string.IsNullOrEmpty(request.filter))
        // {
        //     billQuery = billQuery.Where(x => x.Booking.Customer.UserId == Ulid.Parse(request.filter));
        // }

        //sort
        Expression<Func<Bill, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "billid" => x => x.BillId,
            _ => x => x.BillId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            billQuery = billQuery.OrderByDescending(keySelector);
        }
        else
        {
            billQuery = billQuery.OrderBy(keySelector);
        }


        //paged
        var bills = billQuery
            .Select(a => new BillResponse(
                a.Booking.Customer.UserId,
                a.Booking.Customer.User.LastName,
                a.Booking.Customer.User.FirstName,
                a.Booking.Customer.User.Email,
                a.Booking.Customer.User.Phone,
                a.Order.TableId,
                a.BillId,
                a.CreatedDate,
                a.Booking.BookId,
                (int)a.Booking.BookingPrice / 2,
                a.Booking.BookingDate,
                a.Booking.BookingTime,
                a.Order.OrderId,
                (int)a.Total,
                a.PaymentType,
                a.Order.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    (int)b.Meal.Price,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    (int)b.UnitPrice
                )).ToArray())).AsQueryable();


        var billsList = await PagedList<BillResponse>.CreateAsync(bills, request.page ?? 1, request.pageSize ?? 10);
        return Result<PagedList<BillResponse>>.Success(billsList);
    }
}
