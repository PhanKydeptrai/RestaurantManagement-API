using System.Drawing;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.CategoryDto;
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

        return await _context.Categories.AnyAsync(a => a.CategoryStatus == "Active" && a.CategoryId == id);
        // return true if category status is "Active"
    }


    public async Task SoftDeleteCategory(Ulid id)
    {
        // Cập nhật trạng thái của Category
        await _context.Categories
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CategoryStatus, "InActive"));


        await _context.Meals
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(
                a => a.SetProperty(a => a.MealStatus, "InActive")
                .SetProperty(a => a.SellStatus, "InActive"));
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

    public async Task RestoreCategory(Ulid id)
    {
        // Cập nhật trạng thái của Category
        await _context.Categories
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CategoryStatus, "Active"));

        await _context.Meals
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a
            => a.SetProperty(a => a.MealStatus, "Active")
            .SetProperty(a => a.SellStatus, "Active"));
    }

    public async Task<List<CategoryInfo>> GetAllCategoryInfo()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(a => a.CategoryStatus == "Active")
            .Select(a => new CategoryInfo(a.CategoryId, a.CategoryName))
            .ToListAsync();
    }
}
