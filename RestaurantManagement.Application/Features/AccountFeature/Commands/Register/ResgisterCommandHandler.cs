using MediatR;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.Register;

public class ResgisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _context;
    public ResgisterCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        
        //validation
        var validator = new RegisterCommandValidator(_customerRepository);
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(a => new Error(a.ErrorCode, a.ErrorMessage))
                .ToArray();
            return Result.Failure(errors);
        }
        //TODO: Refactor phương thức cập nhật
        // 1. Sử dụng repository
        // 2. Tối ưu trường hợp khách đăng ký bị chéo thông tin giữa hai cặp tài khoản thường.
        var normalCustomer = await _context.Customers.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.User.Email == request.Email || a.User.Phone == request.Phone);
        if(normalCustomer != null)
        {
            //Update tài khoản
            normalCustomer.CustomerType = "Subscriber";
            normalCustomer.CustomerStatus = "Active";
            normalCustomer.User.Status = "NotActivated";
            normalCustomer.User.FirstName = request.FirstName;
            normalCustomer.User.LastName = request.LastName;
            normalCustomer.User.Gender = request.Gender;
            normalCustomer.User.Password = EncryptProvider.Sha256(request.Password);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }


        //create user
        var user = new User
        {
            UserId = Ulid.NewUlid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Password = EncryptProvider.Sha256(request.Password),
            Gender = request.Gender,
            Status = "NotActivated"
        };

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
        return Result.Success();
    }
}
