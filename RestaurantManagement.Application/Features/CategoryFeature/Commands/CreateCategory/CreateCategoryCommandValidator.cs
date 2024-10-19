using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(ICategoryRepository _categoryRepository)
    {
        RuleFor(p => p.Name)
            .NotNull()
            .WithMessage("Name is required")
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name cannot be longer than 50 characters")
            .Must(a => _categoryRepository.IsCategoryNameExists(a).Result == false)
            .WithMessage("Category name already exists");


    }
}
