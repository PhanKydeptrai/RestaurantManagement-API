using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<RemoveCategoryCommand>
{
    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        //Check status of category
        if (!categoryRepository.CheckStatusOfCategory(Ulid.Parse(request.Id)).Result)
        {
            return Result.Failure(new[] { new Error("Category", "Category not found") });
        }
        //delete
        await categoryRepository.SoftDeleteCategory(Ulid.Parse(request.Id));

        
        #region Decode jwt and system log
        
        //Decode
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        var categoryInfo = await context.Categories.FindAsync(Ulid.Parse(request.Id));
        //Create System Log
        await context.CategoryLogs.AddAsync(new CategoryLog
        {
            UserId = Ulid.Parse(userId),
            CategoryLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} xóa danh mục {categoryInfo.CategoryName}"
        });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
