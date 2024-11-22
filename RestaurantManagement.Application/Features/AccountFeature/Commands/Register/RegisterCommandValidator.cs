using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(p => p.FirstName)
            .NotNull()
            .WithMessage("LastName is required.")
            .NotEmpty()
            .WithMessage("FirstName is required.");

        RuleFor(a => a.LastName)
            .NotNull()
            .WithMessage("LastName is required.")
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(a => a.Email)
            .NotNull()
            .WithMessage("Email is required.")
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is incorrect.")
            .Must(a => customerRepository.IsCustomerEmailExist(a).Result == false)
            .WithMessage("Email is exist");

        RuleFor(a => a.Phone)
            .NotNull()
            .WithMessage("Phonenumber is required.")
            .NotEmpty()
            .WithMessage("Phonenumber is required.")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must(a => customerRepository.IsCustomerPhoneExist(a).Result == false)
            .WithMessage("Phonenumber is exist");

        RuleFor(a => a.Password)
            .NotNull()
            .WithMessage("Password is required.")
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");

        RuleFor(b => b.Gender)
            .NotNull()
            .WithMessage("Gender is required")
            .NotEmpty()
            .WithMessage("Gender must not be empty")
            .Must(b => b == "Male" || b == "Female" || b == "Orther")
            .WithMessage("Gender must be Male, Female or Orther");

    }
}
