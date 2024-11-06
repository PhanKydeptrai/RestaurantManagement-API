using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.MealDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealInfo;

public class GetMealInfoQueryHandler(IApplicationDbContext context) : IQueryHandler<GetMealInfoQuery, List<MealInfo>>
{
    public async Task<Result<List<MealInfo>>> Handle(GetMealInfoQuery request, CancellationToken cancellationToken)
    {
        var meals = context.Meals
            .Where(a => a.MealStatus == "Active")
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(request.searchTerm))
        {
            meals = meals.Where(x => x.MealName.Contains(request.searchTerm));
        }
        
        var mealList = meals.Select(meal => new MealInfo(
                meal.MealId, 
                meal.MealName, 
                meal.Price, 
                meal.ImageUrl)).AsQueryable();
        var mealInfoList = await mealList.ToListAsync();
        return Result<List<MealInfo>>.Success(mealInfoList);
    }
}
