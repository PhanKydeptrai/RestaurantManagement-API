using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, ISystemLogRepository systemLogRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        
        //validation
        var validator = new UpdateCategoryValidator(_categoryRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode,e.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }
        
        _categoryRepository.UpdateCategory(new Category
        {
            CategoryId = request.CategoryId,
            CategoryName = request.CategoryName,
            CategoryStatus = request.CategoryStatus,
            CategoryImage = request.CategoryImage
        });

        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        //await _systemLogRepository.CreateSystemLog(new SystemLog
        //{
        //    SystemLogId = Ulid.NewUlid(),
        //    LogDate = DateTime.Now,
        //    LogDetail = $"Tạo danh mục {request.CategoryName}",
        //    UserId = Ulid.Parse(userId)
        //});
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
