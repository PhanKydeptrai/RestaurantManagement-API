using FluentValidation.Results;
using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        Result<bool> result = new Result<bool>();
    
        //validation
        var validator = new UpdateCategoryValidator(_categoryRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            result.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return result;
        }
        
        Category category = new Category
        {
            CategoryId = request.CategoryId,
            CategoryName = request.CategoryName,
            CategoryStatus = request.CategoryStatus
        };

        _categoryRepository.UpdateCategory(category);
        await _unitOfWork.SaveChangesAsync();
        result.ResultValue = result.IsSuccess = true;

        return result;

    }
}
