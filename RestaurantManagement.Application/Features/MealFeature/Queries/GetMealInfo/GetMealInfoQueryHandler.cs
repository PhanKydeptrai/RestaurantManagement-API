using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.MealDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Queries.GetMealInfo;

public class GetMealInfoQueryHandler : IQueryHandler<GetMealInfoQuery, List<MealInfo>>
{
    private readonly IApplicationDbContext _context;

    public GetMealInfoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MealInfo>>> Handle(GetMealInfoQuery request, CancellationToken cancellationToken)
    {
        var meals = await _context.Meals
            .Where(a => a.MealStatus == "Active")
            .Select(meal => new MealInfo(
                meal.MealId, 
                meal.MealName, 
                meal.Price, 
                meal.ImageUrl))
            .ToListAsync();
        
        return Result<List<MealInfo>>.Success(meals);
    }
}
