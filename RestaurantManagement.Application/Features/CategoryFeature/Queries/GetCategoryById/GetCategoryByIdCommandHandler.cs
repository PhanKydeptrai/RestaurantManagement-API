using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public class GetCategoryByIdCommandHandler : IQueryHandler<GetCategoryByIdCommand, CategoryResponse>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    //REFACOTR
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _context.Categories
            .FindAsync(request.Id);

        if (result == null)
        {
            Error[] a = { new Error("Category", "Category not found") };
            return Result<CategoryResponse>.Failure(a);
        }

        return Result<CategoryResponse>.Success(new CategoryResponse(
                    result.CategoryId,
                    result.CategoryName,
                    result.CategoryStatus,
                    result.ImageUrl));
    }
}
