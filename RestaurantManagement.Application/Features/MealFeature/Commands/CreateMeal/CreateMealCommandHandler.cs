using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
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
    ICategoryRepository categoryRepository,
    IConfiguration configuration,
    IApplicationDbContext context) : ICommandHandler<CreateMealCommand>
{
    public async Task<Result> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new CreateMealCommandValidator(mealRepository, categoryRepository);
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

        #region  decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await context.MealLogs.AddAsync(new MealLog
        // {
        //     LogDate = DateTime.Now,
        //     LogDetails = $"Tạo món {request.MealName}",
        //     MealLogId = Ulid.NewUlid(),
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
