using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.MakePaymentDtos;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetMakePaymentInformation;

public class GetMakePaymentInformationQueryHandler : IQueryHandler<GetMakePaymentInformationQuery, MakePaymentResponse>
{
    private readonly IApplicationDbContext _context;

    public GetMakePaymentInformationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MakePaymentResponse>> Handle(GetMakePaymentInformationQuery request, CancellationToken cancellationToken)
    {

        //validate
        var validator = new GetMakePaymentInformationQueryValidator();
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<MakePaymentResponse>.Failure(errors!);
        }
        MakePaymentResponse? orderResponse = await _context.Orders
            .AsNoTracking()
            .Include(a => a.OrderTransaction)
            .ThenInclude(b => b.Voucher)
            .Include(a => a.OrderDetails)
            .ThenInclude(b => b.Meal)
            .Where(a => a.TableId == int.Parse(request.tableId) && a.PaymentStatus == "Unpaid")
            .Select(a => new MakePaymentResponse(
                a.TableId,
                a.OrderId,
                a.PaymentStatus,
                // (int)a.Total,
                (int)a.OrderTransaction.Amount,
                a.OrderTransaction.Voucher.VoucherCode,
                a.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    (int)b.Meal.Price,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    (int)b.UnitPrice
                )).ToArray()
            )).FirstOrDefaultAsync();

        if (orderResponse == null)
        {
            var error = new[] { new Error("Order", "Order not found") };
            return Result<MakePaymentResponse>.Failure(error);
        }
        return Result<MakePaymentResponse>.Success(orderResponse);
    }
}
