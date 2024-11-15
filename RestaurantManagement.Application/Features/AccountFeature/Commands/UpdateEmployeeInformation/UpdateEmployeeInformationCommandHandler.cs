﻿using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateEmployeeInformation;

public class UpdateEmployeeInformationCommandHandler(
    IUnitOfWork unitOfWork,
    IEmployeeRepository employeeRepository,
    IApplicationDbContext context,
    ISystemLogRepository systemLogRepository) : ICommandHandler<UpdateEmployeeInformationCommand>
{
    public async Task<Result> Handle(UpdateEmployeeInformationCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new UpdateEmployeeInformationCommandValidator(employeeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy user theo id
        var user = await context.Employees
            .Where(a => a.UserId == Ulid.Parse(request.EmployeeId))
            .Select(a => a.User)
            .FirstOrDefaultAsync();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.PhoneNumber;

        if (request.Image != null)
        {
            string oldimageUrl = user.ImageUrl; //Lưu lại ảnh cũ

            //Xử lý lưu ảnh mới
            string newImageUrl = string.Empty;
            if (request.Image != null)
            {
                //tạo memory stream từ file ảnh
                var memoryStream = new MemoryStream();
                await request.Image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                //Upload ảnh lên cloudinary
                var cloudinary = new CloudinaryService();
                var resultUpload = await cloudinary.UploadAsync(memoryStream, request.Image.FileName);
                newImageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary
                user.ImageUrl = newImageUrl;
                //Log                                              
                Console.WriteLine(resultUpload.JsonObj);
            }

            //Xóa ảnh cũ
            if (oldimageUrl != "")
            {
                //Upload ảnh lên cloudinary
                var cloudinary = new CloudinaryService();
                var resultDelete = await cloudinary.DeleteAsync(oldimageUrl);
                //Log
                Console.WriteLine(resultDelete.JsonObj);
            }
        }

        //TODO: Cập nhật system log
        #region Decode token and system log
        // //Decode token
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"{userId} cập nhật thông tin tài khoản",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion


        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

}
