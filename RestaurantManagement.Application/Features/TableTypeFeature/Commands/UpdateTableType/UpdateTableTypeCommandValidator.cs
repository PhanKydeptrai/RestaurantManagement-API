using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;

public class UpdateTableTypeCommandValidator : AbstractValidator<UpdateTableTypeCommand>
{
    public UpdateTableTypeCommandValidator(ITableTypeRepository tableTypeRepository)
    {

        RuleFor(p => p.TableTypeName)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.")
            .Must((name, a) => tableTypeRepository.IsTableTypeNameUnique(a, name.TableTypeId).Result == false)
            .WithMessage("{PropertyName} must be unique.")
            .When(p => tableTypeRepository.IsTableTypeExist(p.TableTypeId).Result == true);


        RuleFor(p => p.TablePrice)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");

    }
}
