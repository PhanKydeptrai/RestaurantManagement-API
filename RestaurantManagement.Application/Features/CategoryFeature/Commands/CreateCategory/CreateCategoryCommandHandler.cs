using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {

        //Validator
        var validator = new CreateCategoryCommandValidator(_categoryRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        //Create Category
        await _categoryRepository.AddCatgory(new Category
        {
            CategoryId = Ulid.NewUlid(),
            CategoryName = request.Name,
            CategoryStatus = "kd",
            ImageUrl = request.Image
        });

        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);

        ////Create System Log
        //await _systemLogRepository.CreateSystemLog(new SystemLog
        //{
        //    SystemLogId = Ulid.NewUlid(),
        //    LogDate = DateTime.Now,
        //    LogDetail = $"Tạo danh mục {request.Name}",
        //    UserId = Ulid.Parse(userId)
        //});


        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
