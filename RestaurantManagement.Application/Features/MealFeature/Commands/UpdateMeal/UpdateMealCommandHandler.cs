using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;
namespace RestaurantManagement.Application.Features.MealFeature.Commands.UpdateMeal;

public class UpdateMealCommandHandler(
    IMealRepository mealRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context,
    ICategoryRepository categoryRepository,
    ISystemLogRepository systemLogRepository) : ICommandHandler<UpdateMealCommand>
{
    public async Task<Result> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        //validator 
        var validator = new UpdateMealValidator(mealRepository, categoryRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Lấy meal theo id  
        var meal = await context.Meals.FindAsync(request.MealId);

        //Update meal
        meal.MealName = request.MealName;
        meal.Price = request.Price;

        meal.Description = request.Description;
        meal.CategoryId = Ulid.Parse(request.CategoryId);

        if (request.Image != null)
        {
            string oldimageUrl = meal.ImageUrl; //Lưu lại ảnh cũ

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
                meal.ImageUrl = newImageUrl;
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

        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Cập nhật thông tin món {request.MealName}",
            UserId = Ulid.Parse(userId)
        });

        await unitOfWork.SaveChangesAsync();




        return Result.Success();
    }
}
