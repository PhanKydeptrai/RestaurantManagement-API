using FluentValidation;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(a => a.id)
            .Must(a => employeeRepository.IsEmployeeExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Employee not found")
            .Custom((id, context) =>
            {
                var token = context.InstanceToValidate.token;
                //Decode jwt
                var claims = JwtHelper.DecodeJwt(token);
                claims.TryGetValue("role", out var role); //Lấy role của người gửi request
                claims.TryGetValue("sub", out var userId); //Lấy userId của người gửi request
                string employeeRole = employeeRepository.GetEmployeeRole(Ulid.Parse(id)).Result;

                if (userId == id) //Kiểm tra xem người gửi request có phải là chính người cần xóa hay không  
                {
                    context.AddFailure("You cant delete yourself");
                }
                else if (role == "Manager" && employeeRole == "Manager" || employeeRole == "Boss")
                {
                    context.AddFailure("You dont have permission to delete this employee");
                }
            })
            .When(e => Ulid.TryParse(e.id, out _))
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid");
    }
}
