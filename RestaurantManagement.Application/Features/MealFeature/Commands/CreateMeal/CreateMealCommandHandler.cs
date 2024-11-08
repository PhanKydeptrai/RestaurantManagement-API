using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.MealFeature.Commands.CreateMeal;

public class CreateMealCommandHandler(
    IUnitOfWork unitOfWork,
    IMealRepository mealRepository,
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    ISystemLogRepository systemLogRepository,
    ICategoryRepository categoryRepository) : ICommandHandler<CreateMealCommand>
{
    public async Task<Result> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        //validation 
        var validator = new CreateMealCommandValidator(mealRepository, categoryRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
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
            var cloudinary = new CloudinaryService();
            var resultUpload = await cloudinary.UploadAsync(memoryStream, request.Image.FileName);
            imageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary
            //Log
            Console.WriteLine(resultUpload.JsonObj);
        }
        
        //Create Meal
        await mealRepository.AddMeal(new Meal
        {
            MealId = Ulid.NewUlid(),
            MealName = request.MealName,
            Price = request.Price,
            ImageUrl = imageUrl,
            Description = request.Description,
            MealStatus = "Active",
            SellStatus = "Active",
            CategoryId = Ulid.Parse(request.CategoryId)
        });

        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await systemLogRepository.CreateSystemLog(new SystemLog
        {
            LogDate = DateTime.Now,
            LogDetail = $"Create Meal {request.MealName}",
            SystemLogId = Ulid.NewUlid(),
            UserId = Ulid.Parse(userId)
        });

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
