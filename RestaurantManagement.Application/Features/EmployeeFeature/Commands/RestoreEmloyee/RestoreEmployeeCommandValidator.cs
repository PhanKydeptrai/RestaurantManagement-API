using FluentValidation;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public class RestoreEmployeeCommandValidator : AbstractValidator<RestoreEmployeeCommand>
{
    public RestoreEmployeeCommandValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(p => p.id)
            .NotNull().WithMessage("{PropertyName} is required.")
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Custom((id, context) =>
            {
                var token = context.InstanceToValidate.token;
                //Decode jwt
                var claims = JwtHelper.DecodeJwt(token);
                claims.TryGetValue("role", out var role);  //Lấy role của người gửi request
                claims.TryGetValue("sub", out var userId); //Lấy userId của người gửi request
                string employeeRole = employeeRepository.GetEmployeeRole(id).Result;

                if (role == "Manager" && employeeRole == "Manager" || employeeRole == "Boss")
                {
                    context.AddFailure("You dont have permission to change this employee");
                }

            });
    }
}
