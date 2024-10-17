using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;

public class CreateMealCommandHandler(
    IUnitOfWork unitOfWork,
    IMealRepository mealRepository,
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    ISystemLogRepository systemLogRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<CreateMealCommand>
{
    public async Task<Result> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        //validation 
        var validator = new CreateMealCommandValidator(mealRepository, categoryRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(p => new Error(p.ErrorCode, p.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }

        //Create Meal
        await mealRepository.AddMeal(new Meal
        {
            MealId = Ulid.NewUlid(),
            MealName = request.MealName,
            Price = request.Price,
            Image = request.Image,
            Description = request.Description,
            MealStatus = "kd",
            SellStatus = "kd",
            CategoryId = request.CategoryId
        });

        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            LogDate = DateTime.Now,
            LogDetail = $"Create Meal {request.MealName}",
            SystemLogId = Ulid.NewUlid(),
            UserId = Ulid.Parse(userId)
        });

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
