using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.UpdateEmployeeRole;

public class UpdateEmployeeRoleCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<UpdateEmployeeRoleCommand>
{
    public async Task<Result> Handle(UpdateEmployeeRoleCommand request, CancellationToken cancellationToken)
    {
        
        var validator = new UpdateEmployeeRoleCommandValidator(employeeRepository);
        //Validate request
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Update Employee Role
        await employeeRepository.UpdateEmployeeRole(Ulid.Parse(request.id), request.role);

        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // var employee = await context.Employees.Include(a => a.User) //Cập nhật chức vụ nhân viên //NOTE: refactor 
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
        //     LogDetails = $"Cập nhật chức vụ nhân viên {employee.FirstName + employee.LastName} thành {request.role}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
