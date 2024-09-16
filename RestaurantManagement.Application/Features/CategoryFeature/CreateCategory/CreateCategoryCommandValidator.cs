using FluentValidation;
using FluentValidation.Validators;
using RestaurantManagement.Domain.Response;

namespace RestaurantManagement.Application.Features.CategoryFeature.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .NotNull()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name cannot be longer than 50 characters");
    }
}
