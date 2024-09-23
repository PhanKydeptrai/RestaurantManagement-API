using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    
    public UpdateCustomerCommandValidator(ICustomerRepository customerRepository)
    {

        //* Rule for last name
        RuleFor(p => p.LastName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 50 characters")
            .Matches("^[a-zA-Z]*$").WithMessage("Last name must not contain numbers or special characters");

        //* Rule for first number
        RuleFor(p => p.FirstName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 50 characters")
            .Matches("^[a-zA-Z]*$").WithMessage("Last name must not contain numbers or special characters");

        //* Rule for phone number
        RuleFor(p => p.PhoneNumber)
            .NotNull().WithMessage("Phone number is required")
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must((command, phoneNumber) => customerRepository
            .IsCustomerPhoneExist_update(command.CustomerId, phoneNumber).Result == false)
            .WithMessage("PhoneNumber number already exists");
            
            
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
            .Must(b => b == "Male" || b == "Female");

    }
}
