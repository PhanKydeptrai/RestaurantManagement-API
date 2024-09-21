using MediatR;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    
    public async Task<CustomerResponse> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        CustomerResponse customer = await _customerRepository.GetCustomerById(request.id);
        
        return customer;
    }
}
