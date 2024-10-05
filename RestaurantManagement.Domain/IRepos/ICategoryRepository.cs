using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ICategoryRepository
{
    //CRUD
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(Ulid id);
    Task AddCatgory(Category category);
    void UpdateCategory(Category category);
    void SoftDeleteCategory(Ulid id);
    void DeleteCategory(Category category);
    //Queries
    IQueryable<Category> GetCategoriesQueryable();
    Task<bool> IsCategoryNameExists(string name);
    Task<bool> IsCategoryNameExistsWhenUpdate(Ulid id,string name);
    Task<bool> CheckStatusOfCategory(Ulid id);
}
