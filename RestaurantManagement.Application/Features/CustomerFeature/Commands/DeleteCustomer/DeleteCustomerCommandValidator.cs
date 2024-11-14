using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(x => x.userId)
            .Must(a => customerRepository.IsCustomerAccountActive(a).Result)
            .WithMessage("Customer account is not active.")
            .When(a => Ulid.TryParse(a.userId, out _))
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid.");
    }
}
