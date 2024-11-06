using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreManyCategory;

public class RestoreManyCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RestoreManyCategoryCommand>
{
    public async Task<Result> Handle(RestoreManyCategoryCommand request, CancellationToken cancellationToken)
    {
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        foreach (var Id in request.id)
        {
            if (await categoryRepository.CheckStatusOfCategory(Id) == true)
            {
                return Result.Failure(new[] { new Error("Category", $"Category {Id} still sell") });
            }

            //Create System Log
            await systemLogRepository.CreateSystemLog(new SystemLog
            {
                UserId = Ulid.Parse(userId),
                SystemLogId = Ulid.NewUlid(),
                LogDate = DateTime.Now,
                LogDetail = $"Khôi phục danh mục {Id}",
            });

            await categoryRepository.SoftDeleteCategory(Id);
        }

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
