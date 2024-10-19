using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository,
        IApplicationDbContext context)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
        _context = context;
    }
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {

        //validation
        var validator = new UpdateCategoryValidator(_categoryRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            Domain.Shared.Error[] errors = validationResult.Errors
                .Select(e => new Domain.Shared.Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        //Lấy category theo id  
        var category = await _context.Categories.FindAsync(request.CategoryId);
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
            var cloudinary = new CloudinaryService();
            var resultUpload = await cloudinary.UploadAsync(memoryStream, request.Image.FileName);
            newImageUrl = resultUpload.SecureUrl.ToString(); //Nhận url ảnh từ cloudinary

            //Log                                              
            Console.WriteLine(resultUpload.JsonObj);
        }

        category.CategoryId = request.CategoryId;
        category.CategoryName = request.CategoryName;
        category.CategoryStatus = request.CategoryStatus;
        category.ImageUrl = newImageUrl;

        var claims = JwtHelper.DecodeJwt(request.Token);
        claims.TryGetValue("sub", out var userId);

        ////Create System Log
        //await _systemLogRepository.CreateSystemLog(new SystemLog
        //{
        //    SystemLogId = Ulid.NewUlid(),
        //    LogDate = DateTime.Now,
        //    LogDetail = $"Tạo danh mục {request.CategoryName}",
        //    UserId = Ulid.Parse(userId)
        //});

        await _unitOfWork.SaveChangesAsync();

        //Xóa ảnh cũ
        if(oldimageUrl != "")
        {
            //Upload ảnh lên cloudinary
            var cloudinary = new CloudinaryService();
            var resultDelete = await cloudinary.DeleteAsync(oldimageUrl);
            //Log
            Console.WriteLine(resultDelete.JsonObj);
        }

        return Result.Success();
    }

}
