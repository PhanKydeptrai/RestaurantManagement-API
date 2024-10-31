using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;

public class CreateEmployeeHandler : ICommandHandler<CreateEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;

    public CreateEmployeeHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFluentEmail fluentEmail, ISystemLogRepository systemLogRepository)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
        _systemLogRepository = systemLogRepository;
    }
    public async Task<Result> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {

        // validate
        var validator = new CreateEmployeeCommandValidator(_employeeRepository);
        var validationResult = await validator.ValidateAsync(request); // Phải dùng thư viện using FluentValidation.Results;
        if (!validationResult.IsValid)
        {
            Error[] errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();

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

        await _userRepository.CreateUser(user);
        await _employeeRepository.CreateEmployee(employee);
        //Deocde jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await _systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Thêm nhân viên {request.FirstName} {request.LastName}",
        //     SystemLogId = Ulid.NewUlid(),
        //     UserId = Ulid.Parse(userId)
        // });

        await _fluentEmail.To(user.Email).Subject("Thông báo thông tin tài khoản")
        .Body($"Thông tin tài khoản nhân viên của bạn: {request.Email} " +
        $"\n Mật Khẩu mặc định: {password}")
        .SendAsync();
        
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}


