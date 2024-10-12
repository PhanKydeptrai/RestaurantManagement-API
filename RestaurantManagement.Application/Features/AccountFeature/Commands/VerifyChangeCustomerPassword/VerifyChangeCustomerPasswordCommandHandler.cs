using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.VerifyChangeCustomerPassword;

public class VerifyChangeCustomerPasswordCommandHandler : ICommandHandler<VerifyChangeCustomerPasswordCommand>
{
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository; 
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public VerifyChangeCustomerPasswordCommandHandler(
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository)
    {
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(VerifyChangeCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        //Lấy token
        var token = await _emailVerificationTokenRepository.GetVerificationTokenById(request.tokenId);

        //kiểm tra token

        if (token == null) //token không tồn tại
        {
            Error[] errors = { new Error("Link", "Đường dẫn không hợp lệ") };
            return Result.Failure(errors);
        }

        if (token.ExpiredDate < DateTime.UtcNow) //token hết hạn
        {
            Error[] errors = { new Error("Link", "Đường dẫn đã hết hạn") }; 
            _emailVerificationTokenRepository.RemoveVerificationToken(token);
            await _unitOfWork.SaveChangesAsync();
            return Result.Failure(errors);
        }

        //token hợp lệ
        //Cập nhật mật khẩu mới
        var user = await _userRepository.GetUserById(token.UserId);
        user.Password = token.Temporary;

        _emailVerificationTokenRepository.RemoveVerificationToken(token);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
