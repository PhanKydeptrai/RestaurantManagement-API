using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email,
        string? UserImage,
        string Role,
        string Gender) : ICommand;
}
