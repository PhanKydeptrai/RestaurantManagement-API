using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository) : ICommandHandler<CreateCategoryCommand>
{
    public async Task<Result> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateCategoryCommandValidator(categoryRepository);

        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
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
            var cloudinary = new CloudinaryService();
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
        //TODO: Cập nhật system log
        #region decode jwt and system log
        // //Decode
        // var claims = JwtHelper.DecodeJwt(request.Token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Tạo danh mục {request.Name}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion


        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
