using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;

public class UpdateTableTypeCommandHandler : ICommandHandler<UpdateTableTypeCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITableTypeRepository _tableTypeRepository;
    public UpdateTableTypeCommandHandler(
        IApplicationDbContext context,
        ITableTypeRepository tableTypeRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository)
    {
        _context = context;
        _tableTypeRepository = tableTypeRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(UpdateTableTypeCommand request, CancellationToken cancellationToken)
    {
        //validation
        var validator = new UpdateTableTypeCommandValidator(_tableTypeRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }

        //Lấy TableType theo id  
        var tableType = await _context.TableTypes.FindAsync(request.TableTypeId);
        //Update TableType
        tableType.TableTypeName = request.TableTypeName;
        tableType.TablePrice = request.TablePrice;
        tableType.Description = request.Description;

        if (request.Image != null)
        {
            string oldimageUrl = tableType.ImageUrl; //Lưu lại ảnh cũ

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
                tableType.ImageUrl = newImageUrl;
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

        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);


        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Tạo danh mục {request.TableTypeId}",
            UserId = Ulid.Parse(userId)
        });
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();


    }
}
