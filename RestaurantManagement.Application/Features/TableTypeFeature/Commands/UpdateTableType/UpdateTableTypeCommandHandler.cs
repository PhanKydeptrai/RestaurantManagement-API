using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableTypeFeature.Commands.UpdateTableType;

public class UpdateTableTypeCommandHandler(
    IApplicationDbContext context,
    ITableTypeRepository tableTypeRepository,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : ICommandHandler<UpdateTableTypeCommand>
{
    public async Task<Result> Handle(UpdateTableTypeCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new UpdateTableTypeCommandValidator(tableTypeRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Lấy TableType theo id  
        var tableType = await context.TableTypes.FindAsync(Ulid.Parse(request.TableTypeId));
        //Update TableType
        tableType.TableTypeName = request.TableTypeName ?? tableType.TableTypeName;
        tableType.TableCapacity = int.Parse(request.TableCapacity);
        tableType.TablePrice = decimal.Parse(request.TablePrice);
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
                var cloudinary = new CloudinaryService(configuration);
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
                var cloudinary = new CloudinaryService(configuration);
                var resultDelete = await cloudinary.DeleteAsync(oldimageUrl);
                //Log
                Console.WriteLine(resultDelete.JsonObj);
            }
        }
        

        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        

        //Create System Log
        await context.TableTypeLogs.AddAsync(new TableTypeLog
        {
            TableTypeLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} cập nhật {request.TableTypeName}",
            UserId = Ulid.Parse(userId)
        });
        #endregion
        await unitOfWork.SaveChangesAsync();

        return Result.Success();


    }
}
