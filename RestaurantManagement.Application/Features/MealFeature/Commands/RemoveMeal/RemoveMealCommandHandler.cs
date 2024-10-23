using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RemoveMeal;

public class RemoveMealCommandHandler : ICommandHandler<RemoveMealCommand>
{
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    public RemoveMealCommandHandler(
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RemoveMealCommand request, CancellationToken cancellationToken)
    {
        var validator = new RemoveMealCommandValidator(_mealRepository);
        var validationResult = await validator.ValidateAsync(request);
        if(!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }
        await _mealRepository.DeleteMeal(request.id);

        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật meal status món {request.id} thành ngừng kinh doanh",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
