using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RemoveCategoryCommand>
{
    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        //Check status of category
        if (!categoryRepository.CheckStatusOfCategory(request.Id).Result)
        {
            return Result.Failure(new[] { new Error("Category", "Category not found") });
        }
        //delete
        await categoryRepository.SoftDeleteCategory(request.Id);

        var claims = JwtHelper.DecodeJwt(request.Token);

        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            UserId = Ulid.Parse(userId),
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Xóa danh mục {request.Id}"
        });
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
