using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, IApplicationDbContext context) : IQueryHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {

        var validator = new GetEmployeeByIdQueryValidator(employeeRepository);
        //validate
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result<EmployeeResponse>.Failure(errors);
        }

        var employee = await context.Employees.Include(a => a.User)
            .Where(a => a.UserId == request.id)
            .Select(a => new EmployeeResponse(
                a.UserId,
                a.User.FirstName,
                a.User.LastName,
                a.User.Email,
                a.User.Phone,
                a.User.Gender,
                a.User.Status,
                a.EmployeeStatus,
                a.Role,
                a.User.ImageUrl
            )).FirstOrDefaultAsync();

        return Result<EmployeeResponse>.Success(employee);
    }
}
