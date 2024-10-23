using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.ChangeSellStatus;

public class ChangeSellStatusCommandHandler : ICommandHandler<ChangeSellStatusCommand>
{
    private readonly IMealRepository _mealRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ChangeSellStatusCommandHandler(
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }


    public async Task<Result> Handle(ChangeSellStatusCommand request, CancellationToken cancellationToken)
    {
        var validator = new ChangeSellStatusCommandValidator(_mealRepository);
        var validationResult = await validator.ValidateAsync(request);

        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }
        
        await _mealRepository.ChangeSellStatus(request.id);
        
        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin trạng thái bán của {request.id} thành ngừng bán",
            UserId = Ulid.Parse(userId)
        });


        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
        
    }
}
