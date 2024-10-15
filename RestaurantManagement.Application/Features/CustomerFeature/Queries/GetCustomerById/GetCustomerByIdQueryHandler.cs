using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : ICommandHandler<GetCustomerByIdQuery,CustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        CustomerResponse customer = await _customerRepository.GetCustomerById(request.id);
        if(customer == null)
        {
            Error[] error = { new Error("Customer", "Customer not found") };
            return Result<CustomerResponse>.Failure(error);
        }
        return Result<CustomerResponse>.Success(customer); 
    }
     
}
