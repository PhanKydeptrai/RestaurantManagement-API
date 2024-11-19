using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public class RemoveMealCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<RemoveMealCommand>
{
    public async Task<Result> Handle(RemoveMealCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new RemoveMealCommandValidator(mealRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        await mealRepository.DeleteMeal(Ulid.Parse(request.id));

        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // var meal = await context.Meals.FindAsync(Ulid.Parse(request.id));
        // //Create System Log
        // await context.MealLogs.AddAsync(new MealLog
        // {
        //     MealLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetails = $"Cập nhật meal status món {meal.MealName} thành ngừng kinh doanh",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
