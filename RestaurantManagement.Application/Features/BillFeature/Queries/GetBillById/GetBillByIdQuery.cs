using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.BillDtos;

namespace RestaurantManagement.Application.Features.BillFeature.Queries.GetBillById;

public record GetBillByIdQuery(string billId) : IQuery<BillResponse>;