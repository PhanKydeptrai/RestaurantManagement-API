using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public class GetCategoryByIdCommandHandler : IRequestHandler<GetCategoryByIdCommand, Result<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryResponse>> Handle(
        GetCategoryByIdCommand request, 
        CancellationToken cancellationToken)
    {
        
        var result = await _context.Categories
            .Select(a => new CategoryResponse(
                a.CategoryId, 
                a.CategoryName, 
                a.CategoryStatus, 
                a.CategoryId + ".jpg"))
            .FirstOrDefaultAsync(a => a.CategoryId == request.Id);

        if(result == null)
        {
            Error[] a = { new Error("Category", "Category not found") };
            return Result<CategoryResponse>.Failure(a);
        }

        return Result<CategoryResponse>.Success(result);
    }
}
