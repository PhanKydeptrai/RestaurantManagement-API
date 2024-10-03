using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Application.Features.Paging;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;

public class CustomerFilterQueryHandler : IRequestHandler<CustomerFilterQuery, PagedList<CustomerResponse>>
{
    private readonly IApplicationDbContext _context;

    public CustomerFilterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<CustomerResponse>> Handle(CustomerFilterQuery request, CancellationToken cancellationToken)
    {
        var customerQuery = _context.Customers.Include(a => a.User).AsQueryable();
        if(!string.IsNullOrEmpty(request.searchTerm))
        {
            customerQuery =  customerQuery.Where(x => x.User.FirstName.Contains(request.searchTerm) || 
                                                    x.User.LastName.Contains(request.searchTerm) ||
                                                    x.User.Email.Contains(request.searchTerm)||
                                                    x.User.Phone.Contains(request.searchTerm));
        }

        var customers = customerQuery.Select(a => new CustomerResponse{
            CustomerId = a.CustomerId,
            FirstName = a.User.FirstName,
            LastName = a.User.LastName,
            Email = a.User.Email,
            PhoneNumber = a.User.Phone,
            Gender = a.User.Gender,
            UserImage = a.User.UserImage
        }).AsQueryable();


        var customerList = await PagedList<CustomerResponse>.CreateAsync(customers, request.page, request.pageSize);
        return customerList;
    }
}
