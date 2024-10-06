using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Ulid id) : IRequest<Result<CustomerResponse>>;

