using FluentValidation;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator(IEmployeeRepository employeeRepository)
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First is Empty.")
                .NotNull().WithMessage("FirstName is Null.")
                .MaximumLength(50).WithMessage("FirstName must not exceed 50 characters.");
                
            RuleFor(a => a.Role)
                .NotEmpty().WithMessage("Role is Empty.")
                .NotNull().WithMessage("Role is Null.")
                .Custom((id, context) =>
                {
                    var token = context.InstanceToValidate.token;
                    var roleRq = context.InstanceToValidate.Role;
                    //Decode jwt
                    var claims = JwtHelper.DecodeJwt(token);
                    claims.TryGetValue("role", out var role); //Lấy role của người gửi request
                    claims.TryGetValue("sub", out var userId); //Lấy userId của người gửi request
                    string employeeRole = employeeRepository.GetEmployeeRole(Ulid.Parse(id)).Result;

                    //Check permission
                    if (role == "Manager" && roleRq == "Manager" || roleRq == "Boss")
                    {
                        context.AddFailure("You dont have permission to update this role");
                    }
                });

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("LastName is Empty.")
                .NotNull().WithMessage("LastName is Null.")
                .MaximumLength(50).WithMessage("LastName must not exceed 50 characters.");

            RuleFor(u => u.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is Empty.")
                .NotNull().WithMessage("PhoneNumber is Null.")
                .Matches(@"^0\d{9}$").WithMessage("PhoneNumber must start with 0 and be exactly 10 digits long.")
                .Must(a => employeeRepository.IsEmployeePhoneExist(a).Result == false)
                .WithMessage("PhoneNumber is already exist.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is Empty.")
                .NotNull().WithMessage("Email is Null.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .Must(a => employeeRepository.IsEmployyeEmailExist(a).Result == false)
                .WithMessage("Email is already exist.");

            //Validate Role
            RuleFor(u => u.Gender)
                .NotNull().WithMessage("Gender is required")
                .NotEmpty().WithMessage("Gender must not be empty")
                .Must(b => b == "Male" || b == "Female" || b == "Orther")
                .WithMessage("Gender must be Male, Female or Orther");
        }
    }
}
