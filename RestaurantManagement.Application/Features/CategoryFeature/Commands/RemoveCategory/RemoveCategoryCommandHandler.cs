using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandHandler : ICommandHandler<RemoveCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        //Check status of category
        if (!_categoryRepository.CheckStatusOfCategory(request.Id).Result)
        {
            return Result.Failure(new[] { new Error("Category", "Category not found") });
        }
        //soft delete
        _categoryRepository.SoftDeleteCategory(request.Id);

        var claims = JwtHelper.DecodeJwt(request.Token);

        claims.TryGetValue("sub", out var userId);
        //Create System Log
        //await _systemLogRepository.CreateSystemLog(new SystemLog
        //{
        //    UserId = Ulid.Parse(userId),
        //    SystemLogId = Ulid.NewUlid(),
        //    LogDate = DateTime.Now,
        //    LogDetail = $"Xóa danh mục {request.Id}",

        //});
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
