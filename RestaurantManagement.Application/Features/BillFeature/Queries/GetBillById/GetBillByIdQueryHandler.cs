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
        var validator = new GetBillByIdQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<BillResponse>.Failure(errors);
        }
        var bill = await context.Bills
            .AsNoTracking()
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
                a.Booking.BookingDate,
                a.Booking.BookingTime,
                a.Order.OrderId,
                a.Total,
                a.PaymentType,
                a.Order.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    b.Meal.Price,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    b.UnitPrice
                )).ToArray()))
                .FirstOrDefaultAsync();

        if (bill != null)
        {
            return Result<BillResponse>.Success(bill);
        }
        return Result<BillResponse>.Failure(new[] { new Error("BillId", "Bill not found") });
    }
}
