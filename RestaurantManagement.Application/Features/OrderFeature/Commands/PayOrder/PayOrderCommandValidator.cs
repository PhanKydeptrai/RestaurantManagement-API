using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.tableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.IsTableExist(a).Result == true)
            .WithMessage("Table does not exist.");

        //Kiểm tra bàn đang có order hay không
        

    }
}
