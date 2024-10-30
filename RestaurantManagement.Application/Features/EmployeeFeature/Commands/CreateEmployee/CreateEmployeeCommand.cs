using Microsoft.AspNetCore.Http;
using RestaurantManagement.Application.Abtractions;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email,
        IFormFile? Image,
        string Role,
        string Gender,
        string token) : ICommand;
}
