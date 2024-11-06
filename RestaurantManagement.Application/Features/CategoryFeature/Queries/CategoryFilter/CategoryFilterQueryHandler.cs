using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.CategoryFilter;

public class CategoryFilterQueryHandler(IApplicationDbContext context) : IQueryHandler<CategoryFilterQuery, PagedList<CategoryResponse>>
{
    public async Task<Result<PagedList<CategoryResponse>>> Handle(CategoryFilterQuery request, CancellationToken cancellationToken)
    {
        var categoriesQuery = context.Categories.AsQueryable();

        //Search
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            categoriesQuery = categoriesQuery.Where(x => x.CategoryName.Contains(request.searchTerm));
        }

        //Filter
        if(!string.IsNullOrEmpty(request.filter))
        {
            categoriesQuery = categoriesQuery.Where(x => x.CategoryStatus == request.filter);
        }


        //sort
        Expression<Func<Category, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "categoryname" => x => x.CategoryName,
            "categoryid" => x => x.CategoryId,
            _ => x => x.CategoryId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            categoriesQuery = categoriesQuery.OrderByDescending(keySelector);
        }
        else
        {
            categoriesQuery = categoriesQuery.OrderBy(keySelector);
        }

        //paged
        var categories = categoriesQuery
            .Select(a => new CategoryResponse(
                a.CategoryId,
                a.CategoryName,
                a.CategoryStatus,
                a.ImageUrl)).AsQueryable();
        var categoriesList = await PagedList<CategoryResponse>.CreateAsync(categories, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<CategoryResponse>>.Success(categoriesList);
    }
}
