using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.RestoreCustomer;

public class RestoreCustomerCommandValidator : AbstractValidator<RestoreCustomerCommand>
{
    public RestoreCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(p => p.userId)
            .NotNull()
            .WithMessage("{PropertyName} is required.")
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("{PropertyName} is not a valid Ulid.")
            .Must(a => customerRepository.IsCustomerIdActive(Ulid.Parse(a)).Result)
            .WithMessage("{PropertyName} is not active.");
    }
}
