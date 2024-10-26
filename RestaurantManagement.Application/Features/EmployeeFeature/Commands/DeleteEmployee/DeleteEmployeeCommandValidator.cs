using FluentValidation;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => employeeRepository.IsEmployeeExist(a).Result == true)
            .WithMessage("Employee not found")
            .Custom((id, context) =>
            {
                var token = context.InstanceToValidate.token;
                //Decode jwt
                var claims = JwtHelper.DecodeJwt(token);
                claims.TryGetValue("role", out var role); //Lấy role của người gửi request
                claims.TryGetValue("sub", out var userId); //Lấy userId của người gửi request
                string employeeRole = employeeRepository.GetEmployeeRole(id).Result;

                if (Ulid.Parse(userId) == id) //Kiểm tra xem người gửi request có phải là chính người cần xóa hay không  
                {
                    context.AddFailure("You cant delete yourself");
                }

                if (role == "Manager" && employeeRole == "Manager" || employeeRole == "Boss")
                {
                    context.AddFailure("You dont have permission to delete this employee");
                }
            });
    }
}
