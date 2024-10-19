using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
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

    public async Task<bool> IsCategoryNameExists(string name)
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

    public async Task<Category?> GetCategoryById(Ulid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public void UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
    }

    public async Task<bool> CheckStatusOfCategory(Ulid id)
    {
        //Check status of category

        return await _context.Categories
                                .Where(x => x.CategoryId == id)
                                .AnyAsync(a => a.CategoryStatus == "kd");
        // return true if category status is "kd"
    }


    public void SoftDeleteCategory(Ulid id)
    {
        var category = _context.Categories.Find(id);
        category.CategoryStatus = "nkd";

    }

    public async Task<bool> IsCategoryNameExistsWhenUpdate(Ulid id, string name)
    {
        return await _context.Categories
                         .AnyAsync(a => a.CategoryName == name && a.CategoryId != id);
    }

    public async Task<bool> IsCategoryExist(Ulid id)
    {
        return await _context.Categories.AnyAsync(a => a.CategoryId == id); 
    }
}
