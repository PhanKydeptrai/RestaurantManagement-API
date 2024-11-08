using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;

public class AssignTableToCustomerCommandValidator : AbstractValidator<AssignTableToCustomerCommand>
{
    public AssignTableToCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.id)            
            .Must(a => tableRepository.GetActiveStatus(int.Parse(a)).Result == "Empty")
            .WithMessage("This table is not empty.")
            .When(a => int.TryParse(a.id, out _))

            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");
        
    }
}
