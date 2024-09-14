namespace RestaurantManagement.Domain.IRepos;

public interface ICategoryRepository
{
    //CRUD
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category> GetCategoryById(Guid id);
    Task AddCatgory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(Category category);
    //Queries
    IQueryable<Category> GetCategoriesQueryable();
    Task<bool> CategoryExists(string name);
}
