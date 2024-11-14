using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFluentEmail fluentEmail, ISystemLogRepository systemLogRepository) : ICommandHandler<CreateEmployeeCommand>
{
    public async Task<Result> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {

        // validate
        var validator = new CreateEmployeeCommandValidator(employeeRepository);
        if(!ValidateRequest.RequestValidator(validator, request, out var errors))
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

        // create new
        string password = RandomStringGenerator.GenerateRandomString(10);
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = EncryptProvider.Sha256(password),
            Phone = request.PhoneNumber,
            ImageUrl = imageUrl,
            Gender = request.Gender,
            Status = "Activated"
        };

        var employee = new Employee
        {
            EmployeeId = Ulid.NewUlid(),
            Role = request.Role,
            EmployeeStatus = "Active",
            UserId = user.UserId,
        };

        await userRepository.CreateUser(user);
        await employeeRepository.CreateEmployee(employee);
        
        //TODO: Cập nhật system log
        #region Decode jwt and system log
        //Deocde jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await _systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Thêm nhân viên {request.FirstName} {request.LastName}",
        //     SystemLogId = Ulid.NewUlid(),
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion
        

        await fluentEmail.To(user.Email).Subject("Nhà hàng Nhum nhum - Thông báo thông tin tài khoản")
        .Body($"Thông tin tài khoản nhân viên của bạn: {request.Email} <br> Mật Khẩu mặc định: {password}")
        .SendAsync();
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}


