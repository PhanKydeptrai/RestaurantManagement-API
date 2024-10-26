using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetAllCategoryId;

public class GetAllCategoryIdQueryHandler : IQueryHandler<GetAllCategoryIdQuery, List<CategoryInfo>>
{
    private readonly IApplicationDbContext _context;
    public GetAllCategoryIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CategoryInfo>>> Handle(GetAllCategoryIdQuery request, CancellationToken cancellationToken)
    {
        var categories = _context.Categories
            .Select(a => new CategoryInfo(a.CategoryId, a.CategoryName))
            .ToList();

        return Result<List<CategoryInfo>>.Success(categories);
    }
}
