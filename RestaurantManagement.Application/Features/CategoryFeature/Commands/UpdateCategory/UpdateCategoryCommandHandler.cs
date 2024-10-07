using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommand<UpdateCategoryCommand>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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
        
        Category category = new Category
        {
            CategoryId = request.CategoryId,
            CategoryName = request.CategoryName,
            CategoryStatus = request.CategoryStatus
        };

        _categoryRepository.UpdateCategory(category);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();

    }
}
