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
                    .Select(a => new CustomerResponse
                    {
                        CustomerId = a.CustomerId,
                        Gender = a.User.Gender,
                        FirstName = a.User.FirstName,
                        LastName = a.User.LastName,
                        Email = a.User.Email,
                        PhoneNumber = a.User.Phone,
                        UserImage = a.User.UserImage
                    }).ToListAsync();


    }
}
