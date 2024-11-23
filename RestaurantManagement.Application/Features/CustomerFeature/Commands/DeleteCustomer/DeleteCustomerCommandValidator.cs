using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(x => x.userId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not valid.")
            .Must(a => customerRepository.IsCustomerIdActive(Ulid.Parse(a)).Result)
            .WithMessage("Customer account is not active.");
    }
}
