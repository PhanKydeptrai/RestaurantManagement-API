using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface IMealRepository
{
    //CRUD
    Task<IEnumerable<Meal>> GetAllMeals();
    Task<Meal?> GetMealById(Guid id);
    Task AddMeal(Meal meal);
    void UpdateMeal(Meal meal);
    void DeleteMeal(Meal meal);

    //Queries
    IQueryable<Meal> GetQueryableOfMeal(); //lấy ra IQueryable của Meal
    Task<IEnumerable<Meal>> GetMealsByCategory(Guid categoryId); // lấy ra danh sách Meal theo CategoryId
    Task<bool> IsMealNameUnique(string name); //Kiểm tra tên món ăn có duy nhất không
    Task<string?> GetMealStatus(Guid id); //Lấy ra trạng thái của món ăn
    Task<string?> GetSellStatus(Guid id); //Lấy ra trạng thái bán của món ăn


}
