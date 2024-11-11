using FluentValidation;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.CreateTable;

public class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator()
    {
        RuleFor(p => p.tableTypeId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid.");

        RuleFor(p => p.quantity)
            // .Must(a => int.Parse(a) > 0)
            // .WithMessage("{PropertyName} must be greater than 0.")
            // .Must(a => int.Parse(a) < 100)
            // .WithMessage("{PropertyName} must be less than 100.")
            // .When(a => int.TryParse(a.quantity, out _))
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");
            // .Must(a => int.TryParse(a, out _))
            // .WithMessage("{PropertyName} is not valid.");

        // RuleFor(p => p.TableTypeId)
        //     .NotEmpty().WithMessage("{PropertyName} is required.");
    }
}
