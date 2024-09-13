using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class MealRepository : IMealRepository
{
    private readonly RestaurantManagementDbContext _context;
    public MealRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddMeal(Meal meal)
    {
        await _context.Meals.AddAsync(meal);
    }

    public void DeleteMeal(Meal meal)
    {
        _context.Meals.Remove(meal);
    }

    public async Task<IEnumerable<Meal>> GetAllMeals()
    {
        return await _context.Meals.ToListAsync();
    }

    public async Task<Meal?> GetMealById(Guid id)
    {
        return await _context.Meals.FirstOrDefaultAsync(m => m.MealId == id);
    }

    public void UpdateMeal(Meal meal)
    {
        _context.Meals.Update(meal);    
    }
}
