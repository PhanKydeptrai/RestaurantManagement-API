using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

public class UpdateEmployeeInformationCommandValidator : AbstractValidator<UpdateEmployeeInformationCommand>
{
    public UpdateEmployeeInformationCommandValidator(IEmployeeRepository employeeRepository)
    {
        //* Rule for last name
        RuleFor(p => p.EmployeeId)
            .NotNull().WithMessage("EmployeeId is required")
            .NotEmpty().WithMessage("EmployeeId is required")
            .Must(a => employeeRepository.IsEmployeeExist(a).Result == true)
            .WithMessage("Employee not found");

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
            .NotNull().WithMessage("Phone number is required")
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be 10 digits long.")
            .Must((command, phoneNumber) => employeeRepository
            .IsEmployeePhoneExist_update(command.EmployeeId, phoneNumber).Result == false)
            .WithMessage("PhoneNumber number already exists");
    }
}
