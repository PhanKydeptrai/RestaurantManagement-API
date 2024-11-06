using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.CustomerFeature.DTOs;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.CustomerFeature.Queries.CustomerFilter;

public class CustomerFilterQueryHandler(IApplicationDbContext context) : IQueryHandler<CustomerFilterQuery, PagedList<CustomerResponse>>
{
    public async Task<Result<PagedList<CustomerResponse>>> Handle(CustomerFilterQuery request, CancellationToken cancellationToken)
    {
        var customerQuery = context.Customers.Include(a => a.User).AsQueryable();
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            customerQuery = customerQuery.Where(x => x.User.FirstName.Contains(request.searchTerm) ||
                                                    x.User.LastName.Contains(request.searchTerm) ||
                                                    x.User.Email.Contains(request.searchTerm) ||
                                                    x.User.Phone.Contains(request.searchTerm));
        }

        //filter bằng customer status
        if (!string.IsNullOrEmpty(request.filterStatus))
        {
            customerQuery = customerQuery.Where(x => x.CustomerStatus == request.filterStatus);
        }

        //filter bằng customer type
        if (!string.IsNullOrEmpty(request.filterUserType))
        {
            customerQuery = customerQuery.Where(x => x.CustomerType == request.filterUserType);
        }

        //filter bằng customer gender
        if (!string.IsNullOrEmpty(request.filterGender))
        {
            customerQuery = customerQuery.Where(x => x.User.Gender == request.filterGender);
        }

        //sort
        Expression<Func<Customer, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "customername" => x => x.User.FirstName + x.User.LastName,
            "customerid" => x => x.UserId,
            _ => x => x.UserId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            customerQuery = customerQuery.OrderByDescending(keySelector);
        }
        else
        {
            customerQuery = customerQuery.OrderBy(keySelector);
        }


        var customers = customerQuery.Select(a => new CustomerResponse(
            a.UserId,
            a.User.FirstName,
            a.User.LastName,
            a.User.Email,
            a.User.Phone,
            a.User.Gender,
            a.User.Status,
            a.CustomerStatus,
            a.CustomerType,
            a.User.ImageUrl)).AsQueryable();

        


        var customerList = await PagedList<CustomerResponse>
            .CreateAsync(customers, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<CustomerResponse>>.Success(customerList);
    }
}
