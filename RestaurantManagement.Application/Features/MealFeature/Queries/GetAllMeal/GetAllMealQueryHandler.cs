using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.MealDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;

public class GetAllMealQueryHandler : IQueryHandler<GetAllMealQuery, PagedList<MealResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetAllMealQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<MealResponse>>> Handle(GetAllMealQuery request, CancellationToken cancellationToken)
    {
        var mealQuery = _context.Meals.Include(x => x.Category).AsQueryable();

        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            mealQuery = mealQuery.Where(x => x.MealName.Contains(request.searchTerm)
            || x.MealStatus.Contains(request.searchTerm));
        }

        //sort
        Expression<Func<Meal, object>> keySelector = request.sortColumn?.ToLower() switch
        {
            "mealname" => x => x.MealName,
            "price" => x => x.Price,
            "mealid" => x => x.MealId,
            _ => x => x.MealId
        };

        if (request.sortOrder?.ToLower() == "desc")
        {
            mealQuery = mealQuery.OrderByDescending(keySelector);
        }
        else
        {
            mealQuery = mealQuery.OrderBy(keySelector);
        }

        //paged
        var meal = mealQuery
            .Select(a => new MealResponse(
                a.MealId,
                a.MealName,
                a.Price,
                a.ImageUrl,
                a.Description,
                a.SellStatus,
                a.MealStatus,
                a.Category.CategoryName));

        var mealList = await PagedList<MealResponse>.CreateAsync(meal, request.page, request.pageSize);

        return Result<PagedList<MealResponse>>.Success(mealList);
    }
}
