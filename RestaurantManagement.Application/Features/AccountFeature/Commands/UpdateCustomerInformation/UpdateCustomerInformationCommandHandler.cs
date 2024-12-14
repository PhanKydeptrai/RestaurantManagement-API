using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    IConfiguration configuration) : ICommandHandler<UpdateCustomerInformationCommand>
{
    public async Task<Result> Handle(UpdateCustomerInformationCommand request, CancellationToken cancellationToken)
    {

        //Validate request
        
        var validator = new UpdateCustomerInformationCommandValidator(customerRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        var user = await context.Customers
            .Include(a => a.User)
            .Where(a => a.UserId == Ulid.Parse(request.CustomerId))
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
                var cloudinary = new CloudinaryService(configuration);
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
                var cloudinary = new CloudinaryService(configuration);
                var resultDelete = await cloudinary.DeleteAsync(oldimageUrl);
                //Log
                Console.WriteLine(resultDelete.JsonObj);
            }
        }
        
     
        #region Decode token and system log
        // //Decode token
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await context.CustomerLogs.AddAsync(new CustomerLog
        // {
        //     CustomerLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetails = $"{userId} cập nhật thông tin tài khoản",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        

        await unitOfWork.SaveChangesAsync();

        
        return Result.Success();
    }
}
