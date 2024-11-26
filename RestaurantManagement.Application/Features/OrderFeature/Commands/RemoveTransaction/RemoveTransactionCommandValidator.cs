using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.RemoveTransaction;

public class RemoveTransactionCommandValidator : AbstractValidator<RemoveTransactionCommand>
{
    public RemoveTransactionCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.tableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => a != null && int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.")
            .Must(a => a != null && tableRepository.IsTableExistAndActive(int.Parse(a)).Result == true)
            .WithMessage("Table does not exist.");
    }
}
