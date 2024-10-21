using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveManyCategory;

public class RemoveManyCategoryCommandHandler : ICommandHandler<RemoveManyCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    public RemoveManyCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RemoveManyCategoryCommand request, CancellationToken cancellationToken)
    {
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);

        foreach (Ulid id in request.id)
        {
            if (await _categoryRepository.CheckStatusOfCategory(id) == false)
            {
                return Result.Failure(new[] { new Error("Category", $"Category {id} not found") });
            }

            //Create System Log
            await _systemLogRepository.CreateSystemLog(new SystemLog
            {
                UserId = Ulid.Parse(userId),
                SystemLogId = Ulid.NewUlid(),
                LogDate = DateTime.Now,
                LogDetail = $"Xóa danh mục {id}",
            });
            _categoryRepository.SoftDeleteCategory(id);
        }
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
