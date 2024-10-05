using System.Data;
using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

// This class is used to validate the CreateCustomerCommand
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{

    public CreateCustomerCommandValidator(ICustomerRepository _customerRepository)
    {
        //* Rule for first name
        RuleFor(b => b.FirstName)
            .NotNull().WithMessage("FirstName is required")
            .NotEmpty().WithMessage("FirstName must not be empty")
            .MaximumLength(10).WithMessage("FirstName must not exceed 10 characters.");

        //* Rule for last name
        RuleFor(b => b.LastName)
            .NotNull().WithMessage("LastName is required")
            .NotEmpty().WithMessage("LastName must not be empty")
            .MaximumLength(10).WithMessage("LastName must not exceed 10 characters.");

        //* Rule for password
        RuleFor(b => b.Password)
            .NotNull().WithMessage("Password is required")
            .NotEmpty().WithMessage("Password must not be empty")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.");

        //* Rule for phone number
        RuleFor(b => b.PhoneNumber)
            .NotNull().WithMessage("PhoneNumber is required")
            .NotEmpty().WithMessage("PhoneNumber must not be empty")
            .MaximumLength(10).WithMessage("PhoneNumber must not exceed 10 characters.")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must(a => _customerRepository.IsCustomerPhoneExist(a).Result == false)
            .WithMessage("PhoneNumber already exists");

        //* Rule for email
        RuleFor(b => b.Email)
            .NotEmpty().WithMessage("Email must not be empty")
            .NotNull().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid")
            .Must(a => _customerRepository.IsCustomerEmailExist(a).Result == false)
            .WithMessage("Email already exists");

        //* Rule for gender
        RuleFor(b => b.Gender)
            .NotNull().WithMessage("Gender is required")
            .NotEmpty().WithMessage("Gender must not be empty") 
            .Must(b => b == "Male" || b == "Female" || b == "Other");
        
    }
}
