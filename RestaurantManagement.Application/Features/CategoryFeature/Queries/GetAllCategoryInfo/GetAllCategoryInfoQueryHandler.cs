using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryInfo;

public class GetAllCategoryIdQueryHandler : IQueryHandler<GetAllCategoryInfoQuery, List<CategoryInfo>>
{
    private readonly ICategoryRepository _categoryRepository;
    public GetAllCategoryIdQueryHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryInfo>>> Handle(GetAllCategoryInfoQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllCategoryInfo();

        return Result<List<CategoryInfo>>.Success(categories);
    }
}
