using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public class UpdateMealInOrderCommandValidator : AbstractValidator<UpdateMealInOrderCommand>
{
    public UpdateMealInOrderCommandValidator(IOrderDetailRepository orderDetailRepository)
    {
        RuleFor(x => x.OrderDetailId)
            .NotNull()
            .WithMessage("OrderDetailId is required.")
            .NotEmpty()
            .WithMessage("OrderDetailId is required.")
            .Must(a => orderDetailRepository.IsOrderDetailCanUpdate(a).Result == true)
            .WithMessage("OrderDetail can't be updated.");

        RuleFor(x => x.Quantity)
            .NotNull()
            .WithMessage("Quantity is required.")
            .NotEmpty()
            .WithMessage("Quantity is required.")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
    }
}
