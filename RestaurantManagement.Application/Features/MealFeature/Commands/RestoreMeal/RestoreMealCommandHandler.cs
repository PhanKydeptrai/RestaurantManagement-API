using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;
public class RestoreMealCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<RestoreMealCommand>
{
    public async Task<Result> Handle(RestoreMealCommand request, CancellationToken cancellationToken)
    {
        var validator = new RestoreMealCommandValidator(mealRepository);
        //validate
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }
        
        await mealRepository.RestoreMeal(Ulid.Parse(request.id));

        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin món {request.id}",
            UserId = Ulid.Parse(userId)
        });

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

