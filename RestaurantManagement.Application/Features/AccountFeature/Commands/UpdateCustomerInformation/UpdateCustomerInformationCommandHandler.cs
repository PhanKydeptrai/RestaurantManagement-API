﻿using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;

public class UpdateCustomerInformationCommandHandler(
    IUnitOfWork unitOfWork,
    ICustomerRepository customerRepository,
    IApplicationDbContext context,
    ISystemLogRepository systemLogRepository) : ICommandHandler<UpdateCustomerInformationCommand>
{
    public async Task<Result> Handle(UpdateCustomerInformationCommand request, CancellationToken cancellationToken)
    {

        //validation
        var validator = new UpdateCustomerInformationCommandValidator(customerRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        var user = await context.Customers
            .Include(a => a.User)
            .Where(a => a.UserId == request.CustomerId)
            .Select(a => a.User)
            .FirstAsync();

        string oldImageUrl = user.ImageUrl; //Lưu lại ảnh cũ

        if (user == null)
        {
            Error[] error = { new Error("Customer", "Customer not found") };
            return Result.Failure(error);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.PhoneNumber;
        user.Gender = request.Gender;

        if (request.UserImage != null) //Nếu có ảnh mới
        {
            string oldimageUrl = user.ImageUrl; //Lưu lại ảnh cũ

            //Xử lý lưu ảnh mới
            string newImageUrl = string.Empty;
            if (request.UserImage != null)
            {
                //tạo memory stream từ file ảnh
                var memoryStream = new MemoryStream();
                await request.UserImage.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                //Upload ảnh lên cloudinary
                var cloudinary = new CloudinaryService();
                var resultUpload = await cloudinary.UploadAsync(memoryStream, request.UserImage.FileName);
                newImageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary
                //Log                                              
                Console.WriteLine(resultUpload.JsonObj);
            }
            user.ImageUrl = newImageUrl;

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

        //Ghi log
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} cập nhật thông tin tài khoản",
            UserId = Ulid.Parse(userId)
        });

        await unitOfWork.SaveChangesAsync();

        
        return Result.Success();
    }
}
