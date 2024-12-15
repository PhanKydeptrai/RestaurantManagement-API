﻿using FluentEmail.Core;
using Microsoft.Extensions.Configuration;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository, 
    IUserRepository userRepository, 
    IUnitOfWork unitOfWork, 
    IFluentEmail fluentEmail,
    IApplicationDbContext context,
    IConfiguration configuration) : ICommandHandler<CreateEmployeeCommand>
{
    public async Task<Result> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {


        var validator = new CreateEmployeeCommandValidator(employeeRepository);
        //Validate request
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Xử lý lưu 
        string imageUrl = string.Empty;
        if (request.Image != null)
        {

            //tạo memory stream từ file ảnh
            var memoryStream = new MemoryStream();
            await request.Image.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            //Upload ảnh lên cloudinary
            var cloudinary = new CloudinaryService(configuration);
            var resultUpload = await cloudinary.UploadAsync(memoryStream, request.Image.FileName);
            imageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary
            //Log
            Console.WriteLine(resultUpload.JsonObj);
        }

        // create new
        string password = RandomStringGenerator.GenerateRandomString(10);
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = EncryptProvider.Sha256(password),
            Phone = request.PhoneNumber,
            ImageUrl = imageUrl ?? string.Empty,
            Gender = request.Gender,
            Status = "Activated"
        };

        var employee = new Employee
        {
            EmployeeId = Ulid.NewUlid(),
            Role = request.Role,
            EmployeeStatus = "Active",
            UserId = user.UserId,
        };

        await userRepository.CreateUser(user);
        await employeeRepository.CreateEmployee(employee);

        #region Decode jwt and system log
        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        //Create System Log
        await context.EmployeeLogs.AddAsync(new EmployeeLog
        {
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} thêm nhân viên {request.FirstName} {request.LastName} chức vụ {request.Role}",
            EmployeeLogId = Ulid.NewUlid(),
            UserId = Ulid.Parse(userId)
        });
        #endregion

        // Gửi email thông báo
        bool emailSent = false;
        int retryCount = 0;
        int maxRetries = 5;

        do
        {
            try
            {
                await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Thông báo thông tin tài khoản")
                    .Body($"Thông tin tài khoản nhân viên của bạn: {request.Email} <br> Mật Khẩu mặc định: {password}")
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


