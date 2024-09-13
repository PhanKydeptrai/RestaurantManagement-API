using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IMealRepository
{
    Task<IEnumerable<Meal>> GetAllMeals();
    Task<Meal?> GetMealById(Guid id);
    Task AddMeal(Meal meal);
    void UpdateMeal(Meal meal);
    void DeleteMeal(Meal meal);
}
