using FluentValidation.Results;
using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.UpdateCategory;

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
        Result<bool> result = new Result<bool>
        {
            Errors = new List<string>(),
            IsSuccess = false,
            ResultValue = false
        };

        //validation
        UpdateCategoryValidator validator = new UpdateCategoryValidator();
        ValidationResult validationResult = validator.Validate(request);
        if(!validationResult.IsValid)
        {
            foreach(var error in validationResult.Errors)
            {
                result.Errors.Add(error.ErrorMessage);
            }
            return result;
        }
        Category category = new Category
        {
            CategoryId = request.CategoryId,
            CategoryName = request.CategoryName,
            CategoryStatus = request.CategoryStatus,
            Desciption = request.Desciption
        };
        
        _categoryRepository.UpdateCategory(category);
        await _unitOfWork.SaveChangesAsync();
        result.ResultValue = result.IsSuccess = true;
        
        return result;
        
    }
}
