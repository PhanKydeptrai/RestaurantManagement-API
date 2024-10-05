using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CategoryDto;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Application.Features.CategoryFeature.Queries.GetCategoryById;

public class GetCategoryByIdCommandHandler : IRequestHandler<GetCategoryByIdCommand, Result<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryByIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdCommand request, CancellationToken cancellationToken)
    {
        Result<CategoryResponse> result = new Result<CategoryResponse>();
        result.ResultValue = await _context.Categories.Select(a => new CategoryResponse(
            a.CategoryId, 
            a.CategoryName, 
            a.CategoryStatus, 
            a.CategoryId + ".jpg")).FirstOrDefaultAsync(a => a.CategoryId == request.Id);

        if (result.ResultValue != null)
        {
            result.IsSuccess = true;
        }

        return result;
    }
}
