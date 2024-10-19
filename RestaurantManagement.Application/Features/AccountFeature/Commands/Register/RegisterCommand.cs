using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Phone,
    string Gender) : ICommand;
