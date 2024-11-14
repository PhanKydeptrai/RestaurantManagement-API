using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.CreateTableType;

public class CreateTableTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ITableTypeRepository tableTypeRepository,
    IApplicationDbContext context,
    ISystemLogRepository systemLogRepository) : ICommandHandler<CreateTableTypeCommand>
{
    public async Task<Result> Handle(CreateTableTypeCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateTableTypeCommandValidator(tableTypeRepository);
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

        //create table type
        await context.TableTypes.AddAsync(
            new TableType
            {
                TableTypeId = Ulid.NewUlid(),
                TableTypeName = request.TableTypeName,
                ImageUrl = imageUrl,
                Status = "Active",
                TableCapacity = request.TableCapacity,
                TablePrice = request.TablePrice,
                Description = request.Description
            });

        //TODO: Cập nhật system log
        #region Decode jwt and system log
        // //decode token
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Tạo danh mục {request.TableTypeName}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
