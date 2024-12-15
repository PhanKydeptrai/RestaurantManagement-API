using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    IFluentEmail fluentEmail,
    IConfiguration configuration) : ICommandHandler<DeleteEmployeeCommand>
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
        
        //FIX: Xử lý hard code
        //gửi mail kích hoạt tài khoản
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            if (configuration["Environment"] == "Development")
            {
                try
                {
                    await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Thông báo sa thải nhân viên")
                        .Body($"Nhân viên {employee.FirstName + " " + employee.LastName} đã bị sa thải khỏi nhà hàng Nhum nhum", isHtml: true)
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
            else
            {
                try
                {
                    #region Send Email using Gmail SMTP
                    // Thông tin đăng nhập và cài đặt máy chủ SMTP
                    string fromEmail = "nhumnhumrestaurant@gmail.com"; // Địa chỉ Gmail của bạn
                    string toEmail = user.Email;  // Địa chỉ người nhận
                    string password = "ekgh lntd brrv bdyj";   // Mật khẩu ứng dụng (nếu bật 2FA) hoặc mật khẩu của tài khoản Gmail
    
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587, // Cổng sử dụng cho TLS
                        Credentials = new NetworkCredential(fromEmail, password), // Đăng nhập vào Gmail
                        EnableSsl = true // Kích hoạt SSL/TLS
                    };
    
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = "Nhà hàng Nhum nhum - Thông báo sa thải nhân viên",
                        Body = $"Nhân viên {employee.FirstName + " " + employee.LastName} đã bị sa thải khỏi nhà hàng Nhum nhum",
                        IsBodyHtml = true // Nếu muốn gửi email ở định dạng HTML
                    };
    
                    mailMessage.To.Add(toEmail);
    
                    // Gửi email
                    smtpClient.Send(mailMessage);
                    #endregion
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
        }
        while (!emailSent && retryCount < maxRetries);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
