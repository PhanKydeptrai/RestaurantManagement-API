using FluentValidation;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;

public class UpdateEmployeeRoleCommandValidator : AbstractValidator<UpdateEmployeeRoleCommand>
{
    public UpdateEmployeeRoleCommandValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(x => x.id)
            .Must(a => employeeRepository.IsEmployeeExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Employee does not exist.")
            .Custom((id, context) =>
            {
                var token = context.InstanceToValidate.token;
                var roleRq = context.InstanceToValidate.role;
                //Decode jwt
                var claims = JwtHelper.DecodeJwt(token);
                claims.TryGetValue("role", out var role); //Lấy role của người gửi request
                claims.TryGetValue("sub", out var userId); //Lấy userId của người gửi request
                string employeeRole = employeeRepository.GetEmployeeRole(Ulid.Parse(id)).Result;

                if (userId == id) //Kiểm tra xem người gửi request có phải là chính người cần cập nhật hay không  
                {
                    context.AddFailure("You cant update yourself");
                }

                else if(role == "Manager" && roleRq == "Manager" || roleRq == "Boss")
                {
                    context.AddFailure("You dont have permission to update this role");
                }

                //manager can't update manager or boss
                else if (role == "Manager" && employeeRole == "Manager" || employeeRole == "Boss")
                {
                    context.AddFailure("You dont have permission to update this employee");
                }
            })
            .When(a => Ulid.TryParse(a.id, out _) == true)
            .NotNull().WithMessage("Employee Id is required.")
            .NotEmpty().WithMessage("Employee Id is required.")
            .Must(a => Ulid.TryParse(a, out _) == true)
            .WithMessage("Employee Id is invalid.");

        RuleFor(x => x.role)
            .NotNull().WithMessage("Employee Id is required.")
            .NotEmpty().WithMessage("Employee Id is required.")
            .Must(a => a == "Receptionist" || a == "Chef" || a == "Manager" || a == "Waiter" || a == "Cashier")
            .WithMessage("Role must be Receptionist, Chef, Manager, Waiter or Cashier");
    }
}
