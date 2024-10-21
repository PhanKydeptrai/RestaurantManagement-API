using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;

public class CreateTableTypeCommandHandler : ICommandHandler<CreateTableTypeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly ITableTypeRepository _tableTypeRepository;
    public CreateTableTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ITableTypeRepository tableTypeRepository,
        IApplicationDbContext context,
        ISystemLogRepository systemLogRepository)
    {
        _unitOfWork = unitOfWork;
        _tableTypeRepository = tableTypeRepository;
        _context = context;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(CreateTableTypeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateTableTypeCommandValidator(_tableTypeRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();

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

        //create table type
        await _context.TableTypes.AddAsync(
            new TableType
            {
                TableTypeId = Ulid.NewUlid(),
                TableTypeName = request.TableTypeName,
                ImageUrl = imageUrl,
                TablePrice = request.TablePrice,
                Description = request.Description
            });

        //decode token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Tạo danh mục {request.TableTypeName}",
            UserId = Ulid.Parse(userId)
        });
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();    
    }
}
