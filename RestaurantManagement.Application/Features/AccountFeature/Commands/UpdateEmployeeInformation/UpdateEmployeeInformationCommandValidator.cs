using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

public class UpdateEmployeeInformationCommandValidator : AbstractValidator<UpdateEmployeeInformationCommand>
{
    public UpdateEmployeeInformationCommandValidator(IEmployeeRepository employeeRepository)
    {
        //* Rule for last name
        RuleFor(p => p.EmployeeId)
            .Must(a => employeeRepository.IsEmployeeExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Employee not found")
            .When(a => Ulid.TryParse(a.EmployeeId, out _))
            .NotNull()
            .WithMessage("EmployeeId is required")
            .NotEmpty()
            .WithMessage("EmployeeId is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("EmployeeId is not valid");

        RuleFor(p => p.LastName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 20 characters");

        //* Rule for first name
        RuleFor(p => p.FirstName)
            .NotNull().WithMessage("Last name is required")
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name must not exceed 20 characters");


        //* Rule for phone number
        RuleFor(p => p.PhoneNumber)
            .Must((command, phoneNumber) => 
            employeeRepository.IsEmployeePhoneExist_update(Ulid.Parse(command.EmployeeId), phoneNumber).Result == false)
            .WithMessage("PhoneNumber number already exists")
            .When(a => Ulid.TryParse(a.EmployeeId, out _))
            .NotNull().WithMessage("Phone number is required")
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.");
    }
}
