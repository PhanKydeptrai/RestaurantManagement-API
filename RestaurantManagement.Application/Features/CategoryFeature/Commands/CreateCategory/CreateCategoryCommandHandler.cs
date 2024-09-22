using MediatR;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

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
            ResultValue = false
        };

        //Validator
        var validator = new CreateCategoryCommandValidator(_categoryRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return result;
        }

        //Create Category
        Category category = new Category
        {
            CategoryId = Guid.NewGuid(),
            CategoryName = request.Name,    
            CategoryStatus = "kd",
            Image = request.Image,
            Desciption = request.Description
        };

        result.ResultValue = true;
        result.IsSuccess = true;
        await _categoryRepository.AddCatgory(category);
        await _unitOfWork.SaveChangesAsync();
        return result;

    }
}
