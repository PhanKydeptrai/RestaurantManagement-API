using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandValidator : AbstractValidator<RestoreTableCommand>
{
    public RestoreTableCommandValidator(ITableRepository tableRepository)
    {
        //TODO: Sửa tên phương thức IsTableJustExist
        RuleFor(a => a.id)
            .Must(a => tableRepository.IsTableJustExist(int.Parse(a)).Result == true)
            .WithMessage("Table does not exist.")
            .Must(a => tableRepository.GetTableStatus(int.Parse(a)).Result == "InActive")
            .WithMessage("Table is still Active.")
            .When(a => int.TryParse(a.id, out _))
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");
            
    }
}
