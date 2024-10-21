using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IMealRepository
{
    //CRUD
    Task<IEnumerable<Meal>> GetAllMeals();
    Task<Meal?> GetMealById(Ulid id);
    Task AddMeal(Meal meal);
    void UpdateMeal(Meal meal);
    Task DeleteMeal(Ulid mealId);
    Task RestoreMeal(Ulid mealId);

    //Queries
    IQueryable<Meal> GetQueryableOfMeal(); //lấy ra IQueryable của Meal
    Task<IEnumerable<Meal>> GetMealsByCategory(Ulid categoryId); // lấy ra danh sách Meal theo CategoryId
    Task<bool> IsMealNameUnique(string name); //Kiểm tra tên món ăn có duy nhất không
    Task<string?> GetMealStatus(Ulid id); //Lấy ra trạng thái của món ăn
    Task<string?> GetSellStatus(Ulid id); //Lấy ra trạng thái bán của món ăn
    Task<bool> IsMealNameUnique_update(Ulid id, string name);
    Task<bool> IsMealExist(Ulid id);

}
