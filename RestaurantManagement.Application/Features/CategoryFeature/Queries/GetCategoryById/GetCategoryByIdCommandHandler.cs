using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Features.CategoryFeature.DTOs;
using RestaurantManagement.Domain.Response;

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
        Result<CategoryResponse> result = new Result<CategoryResponse>
        {
            ResultValue = new CategoryResponse(),
            IsSuccess = false,
            Errors = new List<string>()
        };
        result.ResultValue = await _context.Categories.Select(a => new CategoryResponse
        {
            CategoryId = a.CategoryId,
            CategoryName = a.CategoryName,
            CategoryStatus = a.CategoryStatus,
            Image = a.CategoryId.ToString() + ".jpg", //NOTE: This will be changed to byte[] in the future
            Desciption = a.Desciption
        }).FirstOrDefaultAsync(a => a.CategoryId == request.Id);

        if (result.ResultValue != null)
        {
            result.IsSuccess = true;
        }

        return result;
    }
}
