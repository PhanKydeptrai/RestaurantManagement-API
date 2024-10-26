using System.Data;
using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(a => a.FirstName)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(a => a.LastName)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");

        RuleFor(a => a.Email)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Must(a => customerRepository.IsCustomerEmailExist(a).Result == false)
            .WithMessage("Email is exist")
            .When(a => string.IsNullOrEmpty(a.Phone) && string.IsNullOrWhiteSpace(a.Phone))
            .WithMessage("Either {PropertyName} or Phone is required.");;


        RuleFor(a => a.Phone)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must(a => customerRepository.IsCustomerPhoneExist(a).Result == false)
            .WithMessage("Phonenumber is exist")
            .When(a => string.IsNullOrEmpty(a.Email) && string.IsNullOrWhiteSpace(a.Email))
            .WithMessage("Either {PropertyName} or Email is required.");


        RuleFor(b => b.Gender)
            .NotNull().WithMessage("Gender is required")
            .NotEmpty().WithMessage("Gender must not be empty")
            .Must(b => b == "Male" || b == "Female" || b == "Orther")
            .WithMessage("Gender must be Male, Female or Orther");
    }
}
