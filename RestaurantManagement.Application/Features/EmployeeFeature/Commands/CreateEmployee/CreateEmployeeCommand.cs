using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email,
        byte[]? UserImage,
        string Role,
        string Gender) : ICommand;
}
