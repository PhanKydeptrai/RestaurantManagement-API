using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryInfo;

public class GetAllCategoryIdQueryHandler(
    ICategoryRepository categoryRepository) : IQueryHandler<GetAllCategoryInfoQuery, List<CategoryInfo>>
{
    public async Task<Result<List<CategoryInfo>>> Handle(GetAllCategoryInfoQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllCategoryInfo();

        return Result<List<CategoryInfo>>.Success(categories);
    }
}
