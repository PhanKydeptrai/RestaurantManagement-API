using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandValidator : AbstractValidator<AssignTableToBookedCustomerCommand>
{
    public AssignTableToBookedCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.tableId)
            .Must(a => tableRepository.GetActiveStatus(int.Parse(a)).Result == "Booked")
            .WithMessage("This table is not booked.")
            .When(a => int.TryParse(a.tableId, out _))
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} must be a number.");
    }
}
