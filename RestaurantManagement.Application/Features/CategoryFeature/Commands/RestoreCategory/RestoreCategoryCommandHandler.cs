using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreCategory;

public class RestoreCategoryCommandHandler : ICommandHandler<RestoreCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RestoreCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository,
        ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        var validator = new RestoreCategoryCommandValidator(_categoryRepository);
        var validatorResult = await validator.ValidateAsync(request);
        if (!validatorResult.IsValid)
        {
            var errors = validatorResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        await _categoryRepository.RestoreCategory(request.id);
        //Decode token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            UserId = Ulid.Parse(userId),
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"khôi phục danh mục {request.id}",

        });
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
