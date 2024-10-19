using FluentEmail.Core;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;


namespace RestaurantManagement.Application.Features.EmployeeFeature.Commands.CreateEmployee;

public class CreateEmployeeHandler : ICommandHandler<CreateEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFluentEmail _fluentEmail;

    public CreateEmployeeHandler(IEmployeeRepository employeeRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFluentEmail fluentEmail)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _fluentEmail = fluentEmail;
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
            ImageUrl = request.UserImage,
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

        await _fluentEmail.To(user.Email).Subject("Thông báo thông tin tài khoản")
        .Body($"Thông tin tài khoản nhân viên của bạn: {request.Email} " +
        $"\n Mật Khẩu mặc định: {password}")
        .SendAsync();
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}


