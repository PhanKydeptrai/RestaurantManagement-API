using FluentValidation.Results;
using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Response;
using System.Drawing;

namespace RestaurantManagement.Application.Features.CategoryFeature.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Result<bool> result = new Result<bool>
        {
            IsSuccess = false,
            ResultValue = false,
            Errors = new List<string>()  
        };

        //Validator
        CreateCategoryCommandValidator validator = new CreateCategoryCommandValidator();
        ValidationResult validationResult = validator.Validate(request);
        if(!validationResult.IsValid)
        {
            foreach(var error in validationResult.Errors)
            {
                result.Errors.Add(error.ErrorMessage);
            }
            return result;
        } 

        //Create Category
        Category category = new Category
        {
            CategoryName = request.Name,
            CategoryStatus = "kd"
        };
        result.ResultValue = true;
        result.IsSuccess = true;
        await _categoryRepository.AddCatgory(category);
        await _unitOfWork.SaveChangesAsync();
        return result;

    }
}
