using System.Runtime.CompilerServices;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.DTOs.CustomerDto;
using RestaurantManagement.Domain.DTOs.LoginDto;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Queries.LoginWithGoogle;

public class LoginWithGoogleQueryHandler : IQueryHandler<LoginWithGoogleQuery, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;

    public LoginWithGoogleQueryHandler(
        IApplicationDbContext context,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository,
        IUserRepository userRepository)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<LoginResponse>> Handle(LoginWithGoogleQuery request, CancellationToken cancellationToken)
    {

        try
        {
            var googleUser = await GoogleJsonWebSignature.ValidateAsync(request.token, new GoogleJsonWebSignature.ValidationSettings());

            //TODO: REFACTOR
            var loginResponse = await _context.Customers
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.User.Email == googleUser.Email);

            //Nếu không tìm thấy khách hàng => tạo mới hoặc từ chối
            //1 Tài khoản bị xoá (Từ chối) -
            //2 Tài khoản chưa kích hoạt (Kích hoạt) -
            //3 chưa đăng ký (Đăng ký dùm) -
            #region Xử lý trường hợp không tìm thấy tài khoản
            if (loginResponse == null) //Không thấy
            {
                //Đăng ký tài khoản cho khách
                //create user
                var user = new User
                {
                    UserId = Ulid.NewUlid(),
                    FirstName = googleUser.GivenName,
                    LastName = googleUser.FamilyName ?? string.Empty,
                    Email = googleUser.Email,
                    ImageUrl = googleUser.Picture,
                    Phone = null,
                    Password = null,
                    Gender = null,
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
                    // customer.CustomerType
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

            //Nếu tìm thấy => login hoặc cập nhật
            //1 Khách đã đăng ký trước -
            //2 Khách chỉ đăng ký bằng gg -
            //3 Là khách hàng cũ từng đến ăn nhưng chưa đăng ký -
            #region Xử lý trường hợp khách đã từng đặt bàn nhưng chưa đăng ký
            if (loginResponse != null && loginResponse.CustomerType == "Normal")
            {
                //Update tài khoản
                loginResponse.CustomerType = "Subscriber";
                loginResponse.CustomerStatus = "Active";
                loginResponse.User.Status = "Activated";
                loginResponse.User.FirstName = googleUser.GivenName;
                loginResponse.User.LastName = googleUser.FamilyName;

                await _unitOfWork.SaveChangesAsync();

                var token = _jwtProvider.GenerateJwtToken(
                    loginResponse.UserId.ToString(),
                    loginResponse.User.Email,
                    loginResponse.CustomerType
                );

                return Result<LoginResponse>.Success(new LoginResponse(token, "Subscriber"));
            }
            #endregion


            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Token is invalid") });
        }
        catch (Exception)
        {
            return Result<LoginResponse>.Failure(new[] { new Error("Login Fail", "Token is invalid") });
        }



    }
}
