using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;

public class UpdateCustomerInformationCommandValidator : AbstractValidator<UpdateCustomerInformationCommand>
{
    public UpdateCustomerInformationCommandValidator(ICustomerRepository customerRepository)
    {
        //* Rule for customer id
        RuleFor(a => a.CustomerId)
            .NotNull().WithMessage("Customer Id is required")
            .NotEmpty().WithMessage("Customer Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Customer Id is not valid");


        //* Rule for last name
        RuleFor(p => p.LastName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 20 characters");

        //* Rule for first number
        RuleFor(p => p.FirstName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 20 characters");

        //* Rule for phone number
        RuleFor(p => p.PhoneNumber)
            .Must((command, phoneNumber) => customerRepository
            .IsCustomerPhoneExist_update(Ulid.Parse(command.CustomerId), phoneNumber).Result == false)
            .WithMessage("PhoneNumber number already exists")
            .When(a => Ulid.TryParse(a.CustomerId, out _))

            .NotNull().WithMessage("Phone number is required")
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.");

        

        // ? should customer be able to update email?
        //* Rule for email
        // RuleFor(b => b.Email)
        //     .NotEmpty().WithMessage("Email must not be empty")
        //     .NotNull().WithMessage("Email is required")
        //     .EmailAddress().WithMessage("Email is not valid")
        //     .Must(a => _customerRepository.IsCustomerEmailExist(a).Result == false)
        //     .WithMessage("Email already exists");

        //* Rule for Gender
        RuleFor(b => b.Gender)
            .NotNull().WithMessage("Gender is required")
            .NotEmpty().WithMessage("Gender must not be empty")
            .Must(b => b == "Male" || b == "Female" || b == "Other");
    }
}
