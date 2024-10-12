using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;

public record UpdateCustomerInformationCommand(
    Ulid CustomerId, //* This is the id of the customer to be updated
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    byte[]? UserImage,
    string? Gender,
    string token) : ICommand;
