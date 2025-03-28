using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context,
    IConfiguration configuration) : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {

        //Validate request
        var validator = new UpdateCategoryValidator(categoryRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy category theo id  
        var category = await context.Categories.FindAsync(Ulid.Parse(request.CategoryId));

        category.CategoryName = request.CategoryName;

        //Xử lý ảnh
        if (request.Image != null)
        {
            string oldimageUrl = category.ImageUrl; //Lưu lại ảnh cũ

            //Xử lý lưu ảnh mới
            string newImageUrl = string.Empty;
            if (request.Image != null)
            {
                //tạo memory stream từ file ảnh
                var memoryStream = new MemoryStream();
                await request.Image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                //Upload ảnh lên cloudinary
                var cloudinary = new CloudinaryService(configuration);
                var resultUpload = await cloudinary.UploadAsync(memoryStream, request.Image.FileName);
                newImageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary

                //Log                                              
                Console.WriteLine(resultUpload.JsonObj);
            }
            category.ImageUrl = newImageUrl;

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
       
        #region Decode jwt and system log
        //Decode
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        //Create System Log
        await context.CategoryLogs.AddAsync(new CategoryLog
        {
            CategoryLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} tạo danh mục {request.CategoryName}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
