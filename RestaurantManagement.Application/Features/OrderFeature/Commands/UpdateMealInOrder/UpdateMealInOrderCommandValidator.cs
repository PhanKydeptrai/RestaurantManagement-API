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
            .NotNull()
            .WithMessage("Quantity is required.")
            .NotEmpty()
            .WithMessage("Quantity is required.")
            .Must(a => a != null && int.TryParse(a.ToString(), out _))
            .WithMessage("Quantity is invalid.")
            .Must(a => a != null && int.Parse(a.ToString()) > 0)
            .WithMessage("Quantity must be greater than 0.");
            
    }
}
