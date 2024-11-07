using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;

public record UpdateCustomerInformationCommand(
    string CustomerId, //* This is the id of the customer to be updated
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    IFormFile? UserImage,
    string? Gender,
    string token) : ICommand;
