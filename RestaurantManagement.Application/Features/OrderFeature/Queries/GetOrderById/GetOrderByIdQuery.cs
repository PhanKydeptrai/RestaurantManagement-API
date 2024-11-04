using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.OrderDto;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetOrderById;

public record GetOrderByIdQuery(int tableId) : IQuery<OrderResponse>;

