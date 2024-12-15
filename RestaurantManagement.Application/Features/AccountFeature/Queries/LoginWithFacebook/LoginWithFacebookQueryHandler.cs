using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.LoginDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.LoginWithFacebook;

public class LoginWithFacebookQueryHandler : IQueryHandler<LoginWithFacebookQuery, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;

    public LoginWithFacebookQueryHandler(
        IUserRepository userRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IJwtProvider jwtProvider,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
        _context = context;
    }

    public async Task<Result<LoginResponse>> Handle(LoginWithFacebookQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var loginResponse = await _context.Customers
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.Email == request.email);

            #region Xử lý trường hợp không tìm thấy tài khoản
            if (loginResponse == null) //Không thấy
            {
                //Đăng ký tài khoản cho khách
                //create user
                var user = new User
                {
                    UserId = Ulid.NewUlid(),
                    FirstName = string.Empty,
                    LastName = request.userName ?? string.Empty,
                    Email = request.email,
                    ImageUrl = request.imageUrl,
                    Phone = string.Empty,
                    Password = string.Empty,
                    Gender = string.Empty,
                    Status = "Activated"
                };
                //Create customer
                var customer = new Customer
                {
                    CustomerId = Ulid.NewUlid(),
                    UserId = user.UserId,
                    CustomerStatus = "Active",
                    CustomerType = "Subscriber",
                };

                await _userRepository.CreateUser(user);
                await _customerRepository.CreateCustomer(customer);
                await _unitOfWork.SaveChangesAsync();

                var token = _jwtProvider.GenerateJwtToken(
                    user.UserId.ToString(),
                    user.Email,
                    "Subscriber"
                );

                return Result<LoginResponse>.Success(new LoginResponse(token, "Subscriber"));
            }
            #endregion

            #region Xử lý trường hợp tài khoản bị xoá
            if (loginResponse.User.Status == "Deleted")
            {
                return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Account is deleted") });
            }
            #endregion

            #region Xử lý trường hợp tài khoản chưa kích hoạt
            if (loginResponse.User.Status == "NotActivated" && loginResponse.CustomerType == "Subscriber")
            {
                loginResponse.User.Status = "Activated";
                await _unitOfWork.SaveChangesAsync();

                var token = _jwtProvider.GenerateJwtToken(
                    loginResponse.UserId.ToString(),
                    loginResponse.User.Email,
                    loginResponse.CustomerType
                );

                return Result<LoginResponse>.Success(new LoginResponse(token, "Subscriber"));
            }
            #endregion

            #region Xử lý trường hợp đã được đăng ký trước
            if (loginResponse != null
                && loginResponse.User.Status == "Activated"
                && loginResponse.CustomerType == "Subscriber")
            {
                var token = _jwtProvider.GenerateJwtToken(
                    loginResponse.UserId.ToString(),
                    loginResponse.User.Email,
                    loginResponse.CustomerType
                );

                return Result<LoginResponse>.Success(new LoginResponse(token, "Subscriber"));
            }
            #endregion

            #region Xử lý trường hợp khách đã từng đặt bàn nhưng chưa đăng ký
            if (loginResponse != null && loginResponse.CustomerType == "Normal")
            {
                //Update tài khoản
                loginResponse.CustomerType = "Subscriber";
                loginResponse.CustomerStatus = "Active";
                loginResponse.User.Status = "Activated";
                loginResponse.User.FirstName = string.Empty;
                loginResponse.User.LastName = request.userName ?? string.Empty;

                await _unitOfWork.SaveChangesAsync();

                var token = _jwtProvider.GenerateJwtToken(
                    loginResponse.UserId.ToString(),
                    loginResponse.User.Email,
                    loginResponse.CustomerType
                );

                return Result<LoginResponse>.Success(new LoginResponse(token, "Subscriber"));
            }
            #endregion

            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Login fail") });
        }
        catch (Exception ex)
        {
            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Login fail") });
        }
    }
}
