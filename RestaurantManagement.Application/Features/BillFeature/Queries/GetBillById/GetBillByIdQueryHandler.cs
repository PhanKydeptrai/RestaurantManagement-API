using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.BillDtos;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetBillById;

public class GetBillByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetBillByIdQuery, BillResponse>
{
    public async Task<Result<BillResponse>> Handle(GetBillByIdQuery request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new GetBillByIdQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<BillResponse>.Failure(errors!);
        }

        var bill = await context.Bills
            .AsNoTracking()
            .Where(a => a.BillId == Ulid.Parse(request.billId))
            .Include(a => a.Booking)
            .ThenInclude(a => a.Customer.User)
            .Include(a => a.Order)
            .ThenInclude(a => a.OrderDetails)
            .ThenInclude(a => a.Meal)
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
                (int)a.Booking.BookingPrice/2,
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
                )).ToArray()))
                .FirstOrDefaultAsync();

        

        if (bill != null)
        {
            return Result<BillResponse>.Success(bill);
        }
        return Result<BillResponse>.Failure(new[] { new Error("BillId", "Bill not found") });
    }
}
