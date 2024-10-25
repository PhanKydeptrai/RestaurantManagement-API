using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.RestoreTable;

public class RestoreTableCommandValidator : AbstractValidator<RestoreTableCommand>
{
    public RestoreTableCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("Table does not exist.")
            .Must(a => tableRepository.GetActiveStatus(a).Result == "inactive")
            .WithMessage("Table is still active.");
            
    }
}
