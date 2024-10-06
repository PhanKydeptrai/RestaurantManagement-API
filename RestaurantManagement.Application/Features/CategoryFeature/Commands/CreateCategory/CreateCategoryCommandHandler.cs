using MediatR;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository, 
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
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
        Category category = new Category
        {
            CategoryId = Ulid.NewUlid(),
            CategoryName = request.Name,    
            CategoryStatus = "kd",
            CategoryImage = request.Image
        };

        await _categoryRepository.AddCatgory(category);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
