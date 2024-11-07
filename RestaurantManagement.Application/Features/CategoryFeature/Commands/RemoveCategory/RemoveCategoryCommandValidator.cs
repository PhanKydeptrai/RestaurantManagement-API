using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.RemoveCategory;

public class RemoveCategoryCommandValidator : AbstractValidator<RemoveCategoryCommand>
{
    public RemoveCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(a => a.Id)
            .Must(a => categoryRepository.CheckStatusOfCategory(Ulid.Parse(a)).Result)
            .WithMessage("Category not found")
            .When(a => Ulid.TryParse(a.Id, out _))
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid");
    }
}
