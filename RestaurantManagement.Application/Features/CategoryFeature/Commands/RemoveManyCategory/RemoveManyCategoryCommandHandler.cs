using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveManyCategory;

public class RemoveManyCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RemoveManyCategoryCommand>
{
    public async Task<Result> Handle(RemoveManyCategoryCommand request, CancellationToken cancellationToken)
    {
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);

        foreach (Ulid id in request.id)
        {
            if (await categoryRepository.CheckStatusOfCategory(id) == false)
            {
                return Result.Failure(new[] { new Error("Category", $"Category {id} not found") });
            }

            //Create System Log
            await systemLogRepository.CreateSystemLog(new SystemLog
            {
                UserId = Ulid.Parse(userId),
                SystemLogId = Ulid.NewUlid(),
                LogDate = DateTime.Now,
                LogDetail = $"Xóa danh mục {id}",
            });
            await categoryRepository.SoftDeleteCategory(id);
        }
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
