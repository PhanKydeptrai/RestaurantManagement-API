using FluentValidation;

namespace RestaurantManagement.Application.Features.CategoryFeature.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(p => p.CategoryName)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(50)
            .WithMessage("Category name must not exceed 50 characters.");
        RuleFor(p => p.CategoryStatus)
            .NotEmpty()
            .WithMessage("Category status is required.")
            .Must(p => p == "kd" || p == "nkd");
    }

}
