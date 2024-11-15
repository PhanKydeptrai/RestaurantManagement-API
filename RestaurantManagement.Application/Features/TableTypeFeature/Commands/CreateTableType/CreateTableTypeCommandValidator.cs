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
            .NotNull().WithMessage("{PropertyName} is required.")   
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(p => p != null && decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");

        RuleFor(p => p.TableCapacity)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(p => p != null && int.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be an integer.");
            
    }
}
