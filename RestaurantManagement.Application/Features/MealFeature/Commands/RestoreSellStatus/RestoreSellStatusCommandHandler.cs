using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreSellStatus;

public class RestoreSellStatusCommandHandler(
    IMealRepository mealRepository,
    ISystemLogRepository systemLogRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RestoreSellStatusCommand>
{
    public async Task<Result> Handle(RestoreSellStatusCommand request, CancellationToken cancellationToken)
    {
        var validator = new RestoreSellStatusCommandValidator(mealRepository);
        //TODO: validate
        //validate
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        await mealRepository.RestoreSellStatus(Ulid.Parse(request.id));

        //TODO: Cập nhật system log
        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Cập nhật thông tin trạng thái bán của {request.id} thành bán",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
