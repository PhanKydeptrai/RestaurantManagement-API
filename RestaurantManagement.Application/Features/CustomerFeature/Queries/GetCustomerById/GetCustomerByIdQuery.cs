using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public record GetCustomerByIdQuery(string id) : ICommand<CustomerResponse>;

