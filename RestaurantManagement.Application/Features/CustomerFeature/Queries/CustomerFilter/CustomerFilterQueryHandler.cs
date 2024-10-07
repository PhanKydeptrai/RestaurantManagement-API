using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;

public class CustomerFilterQueryHandler : IQueryHandler<CustomerFilterQuery, PagedList<CustomerResponse>>
{
    private readonly IApplicationDbContext _context;

    public CustomerFilterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<CustomerResponse>>> Handle(CustomerFilterQuery request, CancellationToken cancellationToken)
    {
        var customerQuery = _context.Customers.Include(a => a.User).AsQueryable();
        if(!string.IsNullOrEmpty(request.searchTerm))
        {
            customerQuery =  customerQuery.Where(x => x.User.FirstName.Contains(request.searchTerm) || 
                                                    x.User.LastName.Contains(request.searchTerm) ||
                                                    x.User.Email.Contains(request.searchTerm)||
                                                    x.User.Phone.Contains(request.searchTerm));
        }

        var customers = customerQuery.Select(a => new CustomerResponse(
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
            )).AsQueryable();


        var customerList = await PagedList<CustomerResponse>
            .CreateAsync(customers, request.page, request.pageSize);

        return Result<PagedList<CustomerResponse>>.Success(customerList);
    }
}
