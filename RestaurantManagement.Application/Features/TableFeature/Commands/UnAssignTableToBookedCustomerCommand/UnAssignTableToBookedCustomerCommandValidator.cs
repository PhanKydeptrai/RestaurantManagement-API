using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.UnAssignTableToBookedCustomerCommand;

public class UnAssignTableToBookedCustomerCommandValidator : AbstractValidator<UnAssignTableToBookedCustomerCommand>
{
    public UnAssignTableToBookedCustomerCommandValidator(ITableRepository tableRepository)
    {
        RuleFor(a => a.id)
            .Must(a => tableRepository.IsTableOccupied(int.Parse(a)).Result)
            .WithMessage("Table is invalid")
            .Must(a => tableRepository.IsTableHasUnpaidOrder(int.Parse(a)).Result == false)
            .WithMessage("Table has unpaid order")
            .Must(a => tableRepository.IsTableHasBooking(int.Parse(a)).Result == true)
            .WithMessage("Table has not booking")
            .When(a => int.TryParse(a.id, out _))

            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => int.TryParse(a, out _))
            .WithMessage("{PropertyName} is invalid.");
    }
}
