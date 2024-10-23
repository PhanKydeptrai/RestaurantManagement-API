using FluentValidation;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator()
    {
        RuleFor(p => p.quantity)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
            .LessThan(100).WithMessage("{PropertyName} must be less than 100.");

        // RuleFor(p => p.TableTypeId)
        //     .NotEmpty().WithMessage("{PropertyName} is required.");
    }
}
