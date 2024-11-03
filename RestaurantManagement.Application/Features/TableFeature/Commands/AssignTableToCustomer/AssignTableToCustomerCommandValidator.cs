using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;

public class AssignTableToCustomerCommandValidator : AbstractValidator<AssignTableToCustomerCommand>
{
    public AssignTableToCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.id)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.GetActiveStatus(a).Result == "Empty")
            .WithMessage("This table is not empty.");
        
    }
}
