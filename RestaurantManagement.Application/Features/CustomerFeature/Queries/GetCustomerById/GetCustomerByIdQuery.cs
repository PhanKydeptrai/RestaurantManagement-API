using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Ulid id) : IRequest<CustomerResponse>;

