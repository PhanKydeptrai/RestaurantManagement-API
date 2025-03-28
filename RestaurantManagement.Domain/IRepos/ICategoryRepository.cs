using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ICategoryRepository
{
    //CRUD
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(Ulid id);
    Task AddCatgory(Category category);
    void UpdateCategory(Category category);
    Task SoftDeleteCategory(Ulid id);
    Task RestoreCategory(Ulid id);
    Task<List<CategoryInfo>> GetAllCategoryInfo();
    void DeleteCategory(Category category);
    //Queries
    IQueryable<Category> GetCategoriesQueryable();
    Task<bool> IsCategoryNameExists(string name);
    Task<bool> IsCategoryNameExistsWhenUpdate(Ulid id, string name);
    Task<bool> CheckStatusOfCategory(Ulid id);
    Task<bool> IsCategoryExist(Ulid id);
}
