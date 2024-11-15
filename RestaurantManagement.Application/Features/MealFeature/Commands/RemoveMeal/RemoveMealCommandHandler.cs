using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public class RemoveMealCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RemoveMealCommand>
{
    public async Task<Result> Handle(RemoveMealCommand request, CancellationToken cancellationToken)
    {
        //TODO: validate
        var validator = new RemoveMealCommandValidator(mealRepository);
        var validationResult = await validator.ValidateAsync(request);
        if(!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }
        await mealRepository.DeleteMeal(Ulid.Parse(request.id));

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
        //     LogDetail = $"Cập nhật meal status món {request.id} thành ngừng kinh doanh",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
