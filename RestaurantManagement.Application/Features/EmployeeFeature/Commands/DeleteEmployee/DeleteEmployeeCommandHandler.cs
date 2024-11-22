using FluentEmail.Core;
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
    IApplicationDbContext context,
    IFluentEmail fluentEmail) : ICommandHandler<DeleteEmployeeCommand>
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
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var user = await context.Users.FindAsync(Ulid.Parse(userId)); //Người thực hiện 

        var employee = await context.Employees.Include(a => a.User) //Nhân viên bị xoá //NOTE: refactor 
            .Where(a => a.UserId == Ulid.Parse(request.id))
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


        //Create System Log
        await context.EmployeeLogs.AddAsync(new EmployeeLog
        {
            EmployeeLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{user.FirstName + user.LastName} đã xoá nhân viên {employee.FirstName + employee.LastName} + chức vụ {employee.Role}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        //Delete employee
        await employeeRepository.DeleteEmployee(Ulid.Parse(request.id));
        //gửi mail kích hoạt tài khoản
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            try
            {
                await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Thông báo sa thải nhân viên")
                    .Body("<style>body {font-family: Arial, sans-serif; line-height: 1.6; padding: 20px; background-color: #f9f9f9;} .container {background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);} h2 {color: #333;} p {color: #555;} .footer {margin-top: 20px; font-size: 0.9em; color: #888;}</style> ", isHtml: true)
                    .SendAsync();
                emailSent = true;
            }
            catch
            {
                retryCount++;
                if (retryCount >= maxRetries)
                {
                    return Result.Failure(new[] { new Error("Email", "Failed to send email") });
                }
            }
        }
        while (!emailSent && retryCount < maxRetries);
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
