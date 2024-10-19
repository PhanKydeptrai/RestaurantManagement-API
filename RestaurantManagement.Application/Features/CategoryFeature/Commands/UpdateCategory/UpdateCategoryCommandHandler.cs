using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
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


        var category = await _context.Categories.FindAsync(request.CategoryId);
        string imageId = string.Empty;
        if (category.ImageUrl != "")
        {
            imageId = ExtractPartFromUrl(category.ImageUrl!);
        }
            
        Console.WriteLine(imageId);
        if (category != null)
        {
            category.CategoryId = request.CategoryId;
            category.CategoryName = request.CategoryName;
            category.CategoryStatus = request.CategoryStatus;
            category.ImageUrl = request.ImageUrl;
        }
        

        //Xử lý xóa ảnh
        DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        cloudinary.Api.Secure = true;

        var deleteParams = new DelResParams()
        {
            PublicIds = new List<string> { imageId },
            Type = "upload",
            ResourceType = ResourceType.Image
        };

        var result = cloudinary.DeleteResources(deleteParams);
        Console.WriteLine(result.JsonObj);


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

        return Result.Success();
    }

    public static string ExtractPartFromUrl(string url)
    {
        Uri uri = new Uri(url);
        string path = uri.AbsolutePath;
        string[] segments = path.Split('/');
        string desiredPart = segments[segments.Length - 1].Split('.')[0];
        return desiredPart;
    }
}
