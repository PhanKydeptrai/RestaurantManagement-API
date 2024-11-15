using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;

public class UpdateTableTypeCommandValidator : AbstractValidator<UpdateTableTypeCommand>
{
    public UpdateTableTypeCommandValidator(ITableTypeRepository tableTypeRepository)
    {

        RuleFor(p => p.TableTypeName)
            .Must((name, a) => tableTypeRepository.IsTableTypeNameUnique(a, Ulid.Parse(name.TableTypeId)).Result == false)
            .WithMessage("{PropertyName} must be unique.")
            .When(p => tableTypeRepository.IsTableTypeExist(Ulid.Parse(p.TableTypeId)).Result == true)
            .When(p => Ulid.TryParse(p.TableTypeId, out _))
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("{PropertyName} must not exceed 50 characters.");
            // .Must(a => Ulid.TryParse(a, out _))
            // .WithMessage("{PropertyName} must be a valid Ulid."); //HOTFIX: Kiểm tra sai giá trị

        RuleFor(p => p.Description)
            .MaximumLength(200)
            .WithMessage("{PropertyName} must not exceed 200 characters.");

        RuleFor(p => p.TablePrice)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => decimal.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be a decimal.");

        RuleFor(p => p.TableCapacity)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.")
            .Must(p => p != null && int.TryParse(p.ToString(), out _))
            .WithMessage("{PropertyName} must be an integer.");
    }
}
