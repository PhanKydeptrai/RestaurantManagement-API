using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandValidator : AbstractValidator<AssignTableToBookedCustomerCommand>
{
    public AssignTableToBookedCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(p => p.tableId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => tableRepository.GetActiveStatus(a).Result == "Booked")
            .WithMessage("This table is not booked.");
    }
}
