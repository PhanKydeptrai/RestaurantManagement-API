using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

public record UpdateEmployeeInformationCommand(
    Ulid EmployeeId, //* This is the id of the employee to be updated
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    byte[]? UserImage, string token) : ICommand;
