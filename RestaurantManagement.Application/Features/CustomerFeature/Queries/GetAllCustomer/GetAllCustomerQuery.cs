using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetAllCustomer;

public record GetAllCustomerQuery : IRequest<List<CustomerResponse>>;
