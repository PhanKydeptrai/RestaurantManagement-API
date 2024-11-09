using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.BillDtos;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetAllBill;

public class GetAllBillQueryHandler : IQueryHandler<GetAllBillQuery, PagedList<BillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IApplicationDbContext _context;
    public GetAllBillQueryHandler(
        IBillRepository billRepository,
        IApplicationDbContext context)
    {
        _billRepository = billRepository;
        _context = context;
    }

    public async Task<Result<PagedList<BillResponse>>> Handle(GetAllBillQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetAllBillQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<PagedList<BillResponse>>.Failure(errors);
        }
        
        var billQuery = _context.Bills
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
        if (!string.IsNullOrEmpty(request.filter))
        {
            billQuery = billQuery.Where(x => x.Booking.Customer.UserId == Ulid.Parse(request.filter));
        }


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
                a.Booking.BookId,
                a.Booking.BookingDate,
                a.Booking.BookingTime,
                a.Order.OrderId,
                a.Total,
                a.PaymentType,
                a.Order.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    b.UnitPrice
                )).ToArray())).AsQueryable();

        var billsList = await PagedList<BillResponse>.CreateAsync(bills, request.page ?? 1, request.pageSize ?? 10);
        return Result<PagedList<BillResponse>>.Success(billsList);
    }
}
