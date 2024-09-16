using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.CustomerFeature.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    
    public CreateCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .NotNull()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot be longer than 50 characters");                    

        RuleFor(p => p.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot be longer than 50 characters");

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .NotNull()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(8)
            .WithMessage("Password cannot be longer than 8 characters");
            
        RuleFor(p => p.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .NotNull().WithMessage("Phone number is required")
            .Matches(@"^0\d{9}$").WithMessage("Phone number must be 10 digits long");

        RuleFor(p => p.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .NotNull()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid")
            .Must(p => customerRepository.IsCustomerEmailExist(p).Result == false)
            .WithMessage("Email already exists"); 
    }
}
