using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RestoreManyCategory;

public class RestoreManyCategoryCommandHandler : ICommandHandler<RestoreManyCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    public RestoreManyCategoryCommandHandler(
        ICategoryRepository categoryRepository, 
        IUnitOfWork unitOfWork, 
        ISystemLogRepository systemLogRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RestoreManyCategoryCommand request, CancellationToken cancellationToken)
    {
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        foreach (var Id in request.id)
        {
            

            if (await _categoryRepository.CheckStatusOfCategory(Id) == true)
            {
                return Result.Failure(new[] { new Error("Category", $"Category {Id} still sell") });
            }
            await _systemLogRepository.CreateSystemLog(new SystemLog
            {
                UserId = Ulid.Parse(userId),
                SystemLogId = Ulid.NewUlid(),
                LogDate = DateTime.Now,
                LogDetail = $"Khôi phục danh mục {Id}",
            });

            var category = await _categoryRepository.GetCategoryById(Id);
            category.CategoryStatus = "kd";
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
