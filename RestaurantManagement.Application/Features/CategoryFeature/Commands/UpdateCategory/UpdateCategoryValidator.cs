using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{

    public UpdateCategoryValidator(ICategoryRepository _categoryRepository)
    {
        RuleFor(p => p.CategoryId)
            .NotNull()
            .WithMessage("Category id is required.")
            .NotEmpty()
            .WithMessage("Category id is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Category id is not valid.");

        RuleFor(p => p.CategoryName)
            .Must((id, name) => _categoryRepository.IsCategoryNameExistsWhenUpdate(Ulid.Parse(id.CategoryId), name).Result == false)
            .WithMessage("Category name already exists.")
            .When(a => Ulid.TryParse(a.CategoryId, out _) == true)
            .NotNull()
            .WithMessage("Category name is required.")
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(50)
            .WithMessage("Category name must not exceed 50 characters.");
            

    }

}
