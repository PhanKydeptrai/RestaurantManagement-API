using MediatR;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CustomerFeature.CreateCustomer;
public class CreateCustomerCommand : IRequest<Result<bool>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}
