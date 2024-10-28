using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IQueryHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IApplicationDbContext _context;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, IApplicationDbContext context)
    {
        _employeeRepository = employeeRepository;
        _context = context;
    }

    public async Task<Result<EmployeeResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {

        var validator = new GetEmployeeByIdQueryValidator(_employeeRepository);
        var validationResult = await validator.ValidateAsync(request);
        if(!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result<EmployeeResponse>.Failure(errors);
        }

        var employee = await _context.Employees.Include(a => a.User)
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
