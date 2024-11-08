using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.UpdateMealInOrder;

public class UpdateMealInOrderCommandValidator : AbstractValidator<UpdateMealInOrderCommand>
{
    public UpdateMealInOrderCommandValidator(IOrderDetailRepository orderDetailRepository)
    {
        RuleFor(x => x.OrderDetailId)
            .Must(a => orderDetailRepository.IsOrderDetailCanUpdate(Ulid.Parse(a)).Result == true)
            .WithMessage("OrderDetail can't be updated.")
            .When(a => Ulid.TryParse(a.OrderDetailId, out _))
            .NotNull()
            .WithMessage("OrderDetailId is required.")
            .NotEmpty()
            .WithMessage("OrderDetailId is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("OrderDetailId is invalid.");

        RuleFor(x => x.Quantity)
            .Must(a => int.Parse(a) > 0)
            .WithMessage("Quantity must be greater than 0.")
            .When(a => int.TryParse(a.Quantity, out _))
            .NotNull()
            .WithMessage("Quantity is required.")
            .NotEmpty()
            .WithMessage("Quantity is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("Quantity is invalid.");
            
    }
}
