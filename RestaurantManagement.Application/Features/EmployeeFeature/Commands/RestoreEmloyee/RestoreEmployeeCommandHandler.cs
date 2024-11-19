using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.RestoreEmloyee;

public class RestoreEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    IEmployeeRepository employeeRepository,
    IApplicationDbContext context) : ICommandHandler<RestoreEmployeeCommand>
{
    public async Task<Result> Handle(RestoreEmployeeCommand request, CancellationToken cancellationToken)
    {

        //Validate request
        var validator = new RestoreEmployeeCommandValidator(employeeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        //Restore employee
        await employeeRepository.RestoreEmployee(Ulid.Parse(request.id));
        
        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // var user = await context.Users.FindAsync(Ulid.Parse(userId)); //Người thực hiện 

        // var employee = await context.Employees.Include(a => a.User) //Nhân viên được khôi phục //NOTE: refactor 
        //     .Where(a => a.UserId == Ulid.Parse(request.id))
        //     .Select(a => new EmployeeResponse(
        //         a.UserId,
        //         a.User.FirstName,
        //         a.User.LastName,
        //         a.User.Email,
        //         a.User.Phone,
        //         a.User.Gender,
        //         a.User.Status,
        //         a.EmployeeStatus,
        //         a.Role,
        //         a.User.ImageUrl
        //     )).FirstOrDefaultAsync();

        // //Create System Log
        // await context.EmployeeLogs.AddAsync(new EmployeeLog
        // {
        //     EmployeeLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetails = $"{user.FirstName + user.LastName} đã khôi phục nhân viên {employee.FirstName + employee.LastName} chức vụ {employee.Role}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
