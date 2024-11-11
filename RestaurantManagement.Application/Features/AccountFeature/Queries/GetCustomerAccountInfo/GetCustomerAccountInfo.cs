using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.GetCustomerAccountInfo;

public record GetCustomerAccountInfo() : IQuery<CustomerResponse>;
