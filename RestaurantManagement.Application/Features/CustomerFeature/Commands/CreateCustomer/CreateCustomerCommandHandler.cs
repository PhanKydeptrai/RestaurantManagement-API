using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.CustomerFeature.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IApplicationDbContext _context;
    private readonly IFluentEmail _fluentEmail;
    private readonly IEmailVerify _emailVerify;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ISystemLogRepository systemLogRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IApplicationDbContext context,
        IFluentEmail fluentEmail,
        IEmailVerify emailVerify)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _context = context;
        _fluentEmail = fluentEmail;
        _emailVerify = emailVerify;
    }

    public async Task<Result> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        //Validate 
        var validator = new CreateCustomerCommandValidator(_customerRepository);
        if(!await ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        var normalCustomer = await _context.Customers.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.Phone);

        string randomPassword = RandomStringGenerator.GenerateRandomString(8);
        string randomEncryptPassword = EncryptProvider.Sha256(randomPassword);
        if (normalCustomer != null && normalCustomer.CustomerType != "Subscriber")
        {
            //Update tài khoản
            normalCustomer.CustomerType = "Subscriber";
            normalCustomer.CustomerStatus = "Active";
            normalCustomer.User.Status = "NotActivated";
            normalCustomer.User.FirstName = request.FirstName;
            normalCustomer.User.LastName = request.LastName;
            normalCustomer.User.Gender = request.Gender;
            normalCustomer.User.Password = randomEncryptPassword;
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        //Create User
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            ImageUrl = string.Empty,
            Phone = request.Phone,
            Password = randomEncryptPassword,
            Gender = request.Gender,
            Status = "NotActivated"
        };
        await _userRepository.CreateUser(user);
        // Create Customer
        var customer = new Customer
        {
            CustomerId = Ulid.NewUlid(),
            UserId = user.UserId,
            CustomerStatus = "Active",
            CustomerType = "Subscriber",
        };
        await _customerRepository.CreateCustomer(customer);

        //End

        // Create email verification token
        var emailVerificationToken = new EmailVerificationToken
        {
            EmailVerificationTokenId = Ulid.NewUlid(),
            ExpiredDate = DateTime.UtcNow.AddDays(1),
            UserId = user.UserId,
            CreatedDate = DateTime.UtcNow
        };
        //End

        //gửi mail kích hoạt tài khoản
        var verificationLink = _emailVerify.Create(emailVerificationToken);
        await _fluentEmail.To(user.Email).Subject("Kích hoạt tài khoản")
            .Body($"Vui lòng kích hoạt tài khoản bằng cách click vào link sau: <a href='{verificationLink}'>Click me</a> \n Đây là mật khẩu của bạn: {randomPassword}", isHtml: true)
            .SendAsync();



        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        // Create System Log
        // await _systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Tạo tài khoản cho khách {norm} thành bán",
        //     UserId = Ulid.Parse(userId)
        // });


        await _context.EmailVerificationTokens.AddAsync(emailVerificationToken);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
