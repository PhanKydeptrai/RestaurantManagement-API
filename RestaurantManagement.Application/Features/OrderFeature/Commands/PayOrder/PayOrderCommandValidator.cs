using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.tableId)
            .Must(a => tableRepository.IsTableExist(int.Parse(a)).Result == true)
            .WithMessage("Table does not exist.")
            .When(a => int.TryParse(a.tableId, out _))
            
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");
    }
}
