using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.GetAllCustomer;

public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, List<CustomerResponse>>
{
    private readonly IApplicationDbContext _customerRepository;

    public GetAllCustomerQueryHandler(IApplicationDbContext customerRepository)
    {
        _customerRepository = customerRepository;
    }


    public async Task<List<CustomerResponse>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _customerRepository.Customers
                    .Include(x => x.User)
                    .Where(a => a.User.UserId == a.UserId)
                    .Select(a => new CustomerResponse(
                        a.CustomerId,
                        a.User.FirstName,
                        a.User.LastName,
                        a.User.Email,
                        a.User.Phone,
                        a.User.Gender,
                        a.User.Status,
                        a.CustomerStatus,
                        a.CustomerType,
                        a.User.UserImage
                    )).ToListAsync();


    }
}
