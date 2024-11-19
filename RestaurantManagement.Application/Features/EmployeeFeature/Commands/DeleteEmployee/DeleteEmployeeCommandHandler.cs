using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.DTOs.EmployeeDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(
    IUnitOfWork unitOfWork,
    IEmployeeRepository employeeRepository,
    IApplicationDbContext context) : ICommandHandler<DeleteEmployeeCommand>
{
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new DeleteEmployeeCommandValidator(employeeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        

        #region Decode jwt and system log
        //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // var user = await context.Users.FindAsync(Ulid.Parse(userId)); //Người thực hiện 

        // var employee = await context.Employees.Include(a => a.User) //Nhân viên bị xoá //NOTE: refactor 
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
        //     LogDetails = $"{user.FirstName + user.LastName} đã xoá nhân viên {employee.FirstName + employee.LastName} + chức vụ {employee.Role}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        //Delete employee
        await employeeRepository.DeleteEmployee(Ulid.Parse(request.id));
        //TODO: Gửi mail thông báo đuổi nhân viên

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
