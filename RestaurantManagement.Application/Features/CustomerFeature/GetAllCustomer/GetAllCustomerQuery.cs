using MediatR;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CustomerFeature.GetAllCustomer;

public record GetAllCustomerQuery : IRequest<Result<List<CustomerInformation>>>;

public record CustomerInformation()
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public byte[]? Image { get; set; }
}
