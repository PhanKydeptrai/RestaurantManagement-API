using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;
namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public class UpdateMealCommandHandler : ICommandHandler<UpdateMealCommand>
{
    private readonly IMealRepository _mealRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    public UpdateMealCommandHandler(
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        IApplicationDbContext context,
        ICategoryRepository categoryRepository,
        ISystemLogRepository systemLogRepository)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _context = context;
        _categoryRepository = categoryRepository;
        _systemLogRepository = systemLogRepository;
        
    }

    public async Task<Result> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        //validator 
        var validator = new UpdateMealValidator(_mealRepository, _categoryRepository);
        var validationResult = await validator.ValidateAsync(request);

        if(!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        //Update meal
        Meal meal = (await _context.Meals.FindAsync(request.MealId))!;

        meal.MealName = request.MealName;
        meal.Price = request.Price;
        meal.Image = request.Image;
        meal.Description = request.Description;
        meal.CategoryId = request.CategoryId;


        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin món {request.MealName}",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
