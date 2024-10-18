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

    public async Task<Meal?> GetMealById(Ulid id)
    {
        return await _context.Meals.FindAsync(id);
    }

    public async Task<IEnumerable<Meal>> GetMealsByCategory(Ulid categoryId)
    {
        return await _context.Meals.Where(m => m.CategoryId == categoryId).ToListAsync();
    }

    public async Task<string?> GetMealStatus(Ulid id)
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

    public async Task<string?> GetSellStatus(Ulid id)
    {
        return await _context.Meals.Where(m => m.MealId == id)
                                    .Select(m => m.SellStatus)
                                    .FirstOrDefaultAsync();
    }

    public async Task<bool> IsMealExist(Ulid id)
    {
        return await _context.Meals.AsNoTracking()
            .AnyAsync(m => m.MealId == id);
    }

    public async Task<bool> IsMealNameUnique(string name)
    {
        return await _context.Meals.AsNoTracking().AnyAsync(n => n.MealName == name);
    }

    public async Task<bool> IsMealNameUnique_update(Ulid id,string name)
    {
        return await _context.Meals.AsNoTracking().AnyAsync(n => n.MealName == name && n.MealId != id);
    }
    public void UpdateMeal(Meal meal)
    {
        _context.Meals.Update(meal);    
    }
}
