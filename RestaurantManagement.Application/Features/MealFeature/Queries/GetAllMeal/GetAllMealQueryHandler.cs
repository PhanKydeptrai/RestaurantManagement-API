using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.Paging;
using RestaurantManagement.Domain.DTOs.MealDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Shared;
using System.Linq.Expressions;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetAllMeal;

public class GetAllMealQueryHandler(IApplicationDbContext context) : IQueryHandler<GetAllMealQuery, PagedList<MealResponse>>
{
    public async Task<Result<PagedList<MealResponse>>> Handle(GetAllMealQuery request, CancellationToken cancellationToken)
    {
        var mealQuery = context.Meals.Include(x => x.Category).AsQueryable();

        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            mealQuery = mealQuery.Where(x => x.MealName.Contains(request.searchTerm));
        }
 
        if (!string.IsNullOrEmpty(request.filterMealStatus)) //lọc theo trạng thái kinh doanh
        {
            mealQuery = mealQuery.Where(x => x.MealStatus == request.filterMealStatus);
        }

        if (!string.IsNullOrEmpty(request.filterSellStatus)) //lọc theo trạng thái bán
        {
            mealQuery = mealQuery.Where(x => x.SellStatus == request.filterSellStatus);
        }

        if (!string.IsNullOrEmpty(request.filterCategory)) //Lọc theo category
        {
            mealQuery = mealQuery.Where(x => x.CategoryId == Ulid.Parse(request.filterCategory));
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

        var mealList = await PagedList<MealResponse>.CreateAsync(meal, request.page ?? 1, request.pageSize ?? 10);

        return Result<PagedList<MealResponse>>.Success(mealList);
    }
}
