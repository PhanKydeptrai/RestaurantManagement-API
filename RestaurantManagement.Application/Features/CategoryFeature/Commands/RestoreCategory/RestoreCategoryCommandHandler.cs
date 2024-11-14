using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;

public class RestoreCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<RestoreCategoryCommand>
{
    public async Task<Result> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new RestoreCategoryCommandValidator(categoryRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await categoryRepository.RestoreCategory(request.id);

        #region Decode jwt and system log
        // //Decode token
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     UserId = Ulid.Parse(userId),
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"khôi phục danh mục {request.id}",

        // });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
