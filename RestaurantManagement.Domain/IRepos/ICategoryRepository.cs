using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.IRepos;

public interface ICategoryRepository
{
    //CRUD
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(Guid id);
    Task AddCatgory(Category category);
    void UpdateCategory(Category category);
    void SoftDeleteCategory(Guid id);
    void DeleteCategory(Category category);
    //Queries
    IQueryable<Category> GetCategoriesQueryable();
    Task<bool> IsCategoryNameExists(string name);
    Task<bool> IsCategoryNameExistsWhenUpdate(string name);
    Task<bool> CheckStatusOfCategory(Guid id);
}
