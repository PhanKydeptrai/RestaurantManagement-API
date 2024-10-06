using MediatR;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public byte[] UserImage { get; set; }
        // emp
        public string EmployeeStatus { get; set; }
        public string Role { get; set; }
    }
}
