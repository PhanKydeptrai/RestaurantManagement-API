using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategory;

public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, List<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponse>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
    {
        var categoriesQuery = _categoryRepository.GetCategoriesQueryable();
        var categoryResponses = await categoriesQuery.Select(c => new CategoryResponse
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Image = c.CategoryId.ToString() + ".jpg"
        }).ToListAsync();

        return categoryResponses;
    }
}
