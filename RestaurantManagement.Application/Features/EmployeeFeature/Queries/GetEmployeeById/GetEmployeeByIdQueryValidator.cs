using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(a => a.id)
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => employeeRepository.IsEmployeeExist(a).Result == true)
            .WithMessage("Employee not found");
    }
}
