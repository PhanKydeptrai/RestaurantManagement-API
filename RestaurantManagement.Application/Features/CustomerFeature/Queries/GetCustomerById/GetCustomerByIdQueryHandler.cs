using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository) : ICommandHandler<GetCustomerByIdQuery, CustomerResponse>
{
    public async Task<Result<CustomerResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        
        var validator = new GetCustomerByIdQueryValidator();
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<CustomerResponse>.Failure(errors);
        }

        CustomerResponse? customer = await customerRepository.GetCustomerById(Ulid.Parse(request.id));
        
        if (customer == null)
        {
            Error[] error = { new Error("Customer", "Customer not found") };
            return Result<CustomerResponse>.Failure(error);
        }
        
        return Result<CustomerResponse>.Success(customer);
    }

}
