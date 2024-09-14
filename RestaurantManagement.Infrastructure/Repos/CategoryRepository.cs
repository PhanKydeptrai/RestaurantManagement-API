using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class CategoryRepository : ICategoryRepository
{
    private readonly RestaurantManagementDbContext _context;
    public CategoryRepository(RestaurantManagementDbContext context)
    {
        _context = context;
    }
    public async Task AddCatgory(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public async Task<bool> CategoryExists(string name)
    {
        return await _context.Categories.AnyAsync(n => n.CategoryName == name);
    }

    public void DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    public IQueryable<Category> GetCategoriesQueryable()
    {
        return _context.Categories.AsQueryable();
    }

    public async Task<Category?> GetCategoryById(Guid id)
    {
        return await _context.Categories.FirstOrDefaultAsync(i => i.CategoryId == id);
    }

    public void UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
    }
}
