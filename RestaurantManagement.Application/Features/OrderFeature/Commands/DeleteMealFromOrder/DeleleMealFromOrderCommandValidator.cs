using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.DeleteMealFromOrder;

public class DeleleMealFromOrderCommandValidator : AbstractValidator<DeleleMealFromOrderCommand>
{
    public DeleleMealFromOrderCommandValidator(IOrderDetailRepository orderDetailRepository)
    {
        RuleFor(x => x.id)
            .Must(a => orderDetailRepository.IsOrderDetailCanDelete(Ulid.Parse(a)).Result == true)
            .WithMessage("Order detail can not found.")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotNull()
            .WithMessage("Id is required.")
            .NotEmpty()
            .WithMessage("Id is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid.");
            

        
    }
}
