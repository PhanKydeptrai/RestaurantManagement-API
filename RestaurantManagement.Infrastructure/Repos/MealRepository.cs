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
        return await _context.Meals.FindAsync(id);
    }

    public async Task<IEnumerable<Meal>> GetMealsByCategory(Guid categoryId)
    {
        return await _context.Meals.Where(m => m.CategoryId == categoryId).ToListAsync();
    }

    public async Task<string?> GetMealStatus(Guid id)
    {
        return await _context.Meals.Where(m => m.MealId == id)
                                    .Select(m => m.MealStatus)
                                    .FirstOrDefaultAsync();
    }

    public IQueryable<Meal> GetQueryableOfMeal()
    {
        IQueryable<Meal> meals = _context.Meals;
        return meals;
    }

    public async Task<string?> GetSellStatus(Guid id)
    {
        return await _context.Meals.Where(m => m.MealId == id)
                                    .Select(m => m.SellStatus)
                                    .FirstOrDefaultAsync();
    }

    public Task<bool> IsMealNameUnique(string name)
    {
        return _context.Meals.AnyAsync(n => n.MealName == name);
    }

    public void UpdateMeal(Meal meal)
    {
        _context.Meals.Update(meal);    
    }
}
