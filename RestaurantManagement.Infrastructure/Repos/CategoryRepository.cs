using System.Drawing;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Infrastructure.Persistence;

namespace RestaurantManagement.Infrastructure.Repos;

public class CategoryRepository(RestaurantManagementDbContext context) : ICategoryRepository
{
    public async Task AddCatgory(Category category)
    {
        await context.Categories.AddAsync(category);
    }

    public async Task<bool> IsCategoryNameExists(string name)
    {
        return await context.Categories.AnyAsync(n => n.CategoryName == name);
    }

    public void DeleteCategory(Category category)
    {
        context.Categories.Remove(category);
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await context.Categories.ToListAsync();
    }

    public IQueryable<Category> GetCategoriesQueryable()
    {
        return context.Categories.AsQueryable();
    }

    public async Task<Category?> GetCategoryById(Ulid id)
    {
        return await context.Categories.FindAsync(id);
    }

    public void UpdateCategory(Category category)
    {
        context.Categories.Update(category);
    }

    public async Task<bool> CheckStatusOfCategory(Ulid id)
    {
        //Check status of category

        return await context.Categories.AnyAsync(a => a.CategoryStatus == "Active" && a.CategoryId == id);
        // return true if category status is "Active"
    }


    public async Task SoftDeleteCategory(Ulid id)
    {
        // Cập nhật trạng thái của Category
        await context.Categories
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CategoryStatus, "InActive"));


        await context.Meals
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(
                a => a.SetProperty(a => a.MealStatus, "InActive")
                .SetProperty(a => a.SellStatus, "InActive"));
    }

    public async Task<bool> IsCategoryNameExistsWhenUpdate(Ulid id, string name)
    {
        return await context.Categories
                         .AnyAsync(a => a.CategoryName == name && a.CategoryId != id);
    }

    public async Task<bool> IsCategoryExist(Ulid id)
    {
        return await context.Categories.AnyAsync(a => a.CategoryId == id);
    }

    public async Task RestoreCategory(Ulid id)
    {
        // Cập nhật trạng thái của Category
        await context.Categories
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a => a.SetProperty(a => a.CategoryStatus, "Active"));

        await context.Meals
            .Where(a => a.CategoryId == id)
            .ExecuteUpdateAsync(a
            => a.SetProperty(a => a.MealStatus, "Active")
            .SetProperty(a => a.SellStatus, "Active"));
    }

    public async Task<List<CategoryInfo>> GetAllCategoryInfo()
    {
        return await context.Categories
            .AsNoTracking()
            .Where(a => a.CategoryStatus == "Active")
            .Select(a => new CategoryInfo(a.CategoryId, a.CategoryName))
            .ToListAsync();
    }
}
