using FluentValidation;
using RestaurantManagement.Domain.IRepos;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryValidator : AbstractValidator<GetEmployeeByIdQuery>
{
    public GetEmployeeByIdQueryValidator(IEmployeeRepository employeeRepository)
    {
        RuleFor(a => a.id)
            .Must(a => employeeRepository.IsEmployeeExist(Ulid.Parse(a)).Result == true)
            .WithMessage("Employee not found")
            .When(a => Ulid.TryParse(a.id, out _))
            .NotNull()
            .WithMessage("Id is required")
            .NotEmpty()
            .WithMessage("Id is required")
            .Must(a => Ulid.TryParse(a, out _))
            .WithMessage("Id is not valid");
    }
}
