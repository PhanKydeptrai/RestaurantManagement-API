using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;

public class RestoreCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ICategoryRepository categoryRepository,
    IApplicationDbContext context) : ICommandHandler<RestoreCategoryCommand>
{
    public async Task<Result> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new RestoreCategoryCommandValidator(categoryRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        //NOTE: Need refactor
        var category = await categoryRepository.GetCategoryById(request.id); //Get category by id to get category name
        
        await categoryRepository.RestoreCategory(request.id);

        #region Decode jwt and system log
        //Decode token
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await context.CategoryLogs.AddAsync(new CategoryLog
        // {
        //     UserId = Ulid.Parse(userId),
        //     CategoryLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetails = $"khôi phục danh mục {category.CategoryName}",

        // });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
