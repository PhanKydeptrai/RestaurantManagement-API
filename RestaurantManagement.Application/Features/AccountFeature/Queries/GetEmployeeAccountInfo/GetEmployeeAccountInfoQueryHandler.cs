using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.GetEmployeeAccountInfo;

public class GetEmployeeAccountInfoQueryHandler : IQueryHandler<GetEmployeeAccountInfoQuery, EmployeeResponse>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeAccountInfoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeResponse>> Handle(GetEmployeeAccountInfoQuery request, CancellationToken cancellationToken)
    {
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        
        //Get employee info
        var employeeInfo = await _context.Employees.Include(a => a.User).Where(a => a.UserId == Ulid.Parse(userId))
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
        
        return Result<EmployeeResponse>.Success(employeeInfo);
    }
}
