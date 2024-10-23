using System.IdentityModel.Tokens.Jwt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.RestoreMeal;
public class RestoreMealCommandHandler : ICommandHandler<RestoreMealCommand>
{
    private readonly IMealRepository _mealRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RestoreMealCommandHandler(
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
        
        
    }

    public async Task<Result> Handle(RestoreMealCommand request, CancellationToken cancellationToken)
    {
        var validator = new RestoreMealCommandValidator(_mealRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);

        }

        await _mealRepository.RestoreMeal(request.id);
        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin món {request.id}",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

