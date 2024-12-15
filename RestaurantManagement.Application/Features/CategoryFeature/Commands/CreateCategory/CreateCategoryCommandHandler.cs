using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context,
    IConfiguration configuration) : ICommandHandler<CreateCategoryCommand>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new CreateCategoryCommandValidator(categoryRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }


        //Xử lý ảnh
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


        //Create Category
        await categoryRepository.AddCatgory(new Category
        {
            CategoryId = Ulid.NewUlid(),
            CategoryName = request.Name,
            CategoryStatus = "Active",
            ImageUrl = imageUrl
        });

        
        #region decode jwt and system log
        //Decode
        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await context.CategoryLogs.AddAsync(new CategoryLog
        {
            CategoryLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Tạo danh mục {request.Name}",
            UserId = Ulid.Parse(userId)
        });
        #endregion


        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
