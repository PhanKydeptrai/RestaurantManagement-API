using RestaurantManagement.Application.Abtractions;
namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string Gender,
    string token
) : ICommand;

