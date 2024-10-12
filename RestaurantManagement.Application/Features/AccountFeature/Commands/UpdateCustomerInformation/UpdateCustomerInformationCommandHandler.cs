using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.AccountFeature.Commands.UpdateCustomerInformation;

public class UpdateCustomerInformationCommandHandler : ICommandHandler<UpdateCustomerInformationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly IApplicationDbContext _context;
    private readonly ISystemLogRepository _systemLogRepository;
    public UpdateCustomerInformationCommandHandler(
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository,
        IApplicationDbContext context,
        ISystemLogRepository systemLogRepository)
    {
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _context = context;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<Result> Handle(UpdateCustomerInformationCommand request, CancellationToken cancellationToken)
    {

        //validation
        var validator = new UpdateCustomerInformationCommandValidator(_customerRepository);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            return Result.Failure(errors);
        }

        var user = await _context.Customers
            .Include(a => a.User)
            .Where(a => a.CustomerId == request.CustomerId)
            .Select(a => a.User).FirstAsync();

        if (user == null)
        {
            Error[] error = { new Error("Customer", "Customer not found") };
            return Result.Failure(error);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.PhoneNumber;
        user.Gender = request.Gender;
        user.UserImage = request.UserImage ?? user.UserImage;
        //? Should we add a email field to the UpdateCustomerCommand class?
        //Ghi log
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"{userId} cập nhật thông tin tài khoản",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
