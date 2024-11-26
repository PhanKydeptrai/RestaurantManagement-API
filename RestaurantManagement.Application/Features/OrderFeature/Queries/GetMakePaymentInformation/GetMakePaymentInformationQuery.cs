using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.MakePaymentDtos;

namespace RestaurantManagement.Application.Features.OrderFeature.Queries.GetMakePaymentInformation;

public record GetMakePaymentInformationQuery(string tableId) : IQuery<MakePaymentResponse>;

