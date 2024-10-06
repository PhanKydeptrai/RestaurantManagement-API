using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Ulid CustomerId, //* This is the id of the customer to be updated
    string? FirstName, 
    string? LastName, 
    string? PhoneNumber, 
    byte[]? UserImage, 
    string? Gender) : IRequest<Result>;

