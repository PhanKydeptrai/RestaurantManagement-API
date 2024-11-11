using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.OrderDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    IApplicationDbContext context) : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        OrderResponse? orderResponse = await context.Orders
            .AsNoTracking()
            .Include(a => a.OrderDetails)
            .ThenInclude(b => b.Meal)
            .Where(a => a.TableId == request.tableId && a.PaymentStatus == "Unpaid")
            .Select(a => new OrderResponse(
                a.TableId,
                a.OrderId,
                a.PaymentStatus,
                a.Total,
                a.OrderDetails.Select(b => new OrderDetailResponse(
                    b.OrderDetailId,
                    b.MealId,
                    b.Meal.MealName,
                    b.Meal.Price,
                    b.Meal.ImageUrl,
                    b.Quantity,
                    b.UnitPrice
                )).ToArray()
            )).FirstOrDefaultAsync();

        if(orderResponse == null)
        {
            var errors = new[] { new Error("Order", "Order not found") };
            return Result<OrderResponse>.Failure(errors);
        }

        return Result<OrderResponse>.Success(orderResponse);
    }
}
