using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public class DeleleMealFromOrderCommandValidator : AbstractValidator<DeleleMealFromOrderCommand>
{
    public DeleleMealFromOrderCommandValidator(IOrderDetailRepository orderDetailRepository)
    {
        RuleFor(x => x.id)
            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => orderDetailRepository.IsOrderDetailCanDelete(a).Result == true)
            .WithMessage("Order detail can not found.");
            

        
    }
}
