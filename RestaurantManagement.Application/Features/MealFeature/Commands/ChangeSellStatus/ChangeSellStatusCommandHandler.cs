using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public class ChangeSellStatusCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<ChangeSellStatusCommand>
{
    public async Task<Result> Handle(ChangeSellStatusCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new ChangeSellStatusCommandValidator(mealRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await mealRepository.ChangeSellStatus(Ulid.Parse(request.id));

        //TODO: Cập nhật system log
        #region decode jwt and system log
        // //Deocde jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Cập nhật thông tin trạng thái bán của {request.id} thành ngừng bán",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion


        await unitOfWork.SaveChangesAsync();
        return Result.Success();

    }
}
