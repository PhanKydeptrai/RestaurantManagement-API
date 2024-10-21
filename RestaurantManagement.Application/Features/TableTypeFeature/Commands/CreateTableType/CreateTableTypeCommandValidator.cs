using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;

public class CreateTableTypeCommandValidator : AbstractValidator<CreateTableTypeCommand>
{
    public CreateTableTypeCommandValidator(ITableTypeRepository tableTypeRepository)
    {
        RuleFor(p => p.TableTypeName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must(a => tableTypeRepository.IsTableTypeNameUnique(a).Result == false)
            .WithMessage("{PropertyName} name is not unique");

        RuleFor(p => p.TablePrice)
            .Must(p => p is decimal)
            .WithMessage("{PropertyName} must be a decimal.")
            .When(p => p != null).WithMessage("{PropertyName} must be a decimal.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.");

        
    }
}
