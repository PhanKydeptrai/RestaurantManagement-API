using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

public class CreateVoucherCommandHanler : ICommandHandler<CreateVoucherCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemLogRepository _systemLogRepository;
    public CreateVoucherCommandHanler(
        IApplicationDbContext context,
        IVoucherRepository voucherRepository,
        ISystemLogRepository systemLogRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _voucherRepository = voucherRepository;
        _systemLogRepository = systemLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateVoucherCommandValidator(_voucherRepository);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(a => new Error(a.ErrorCode, a.ErrorMessage)).ToArray();
            return Result.Failure(errors);
        }
        
        //create voucher
        await _context.Vouchers.AddAsync(new Voucher
        {
            VoucherId = Ulid.NewUlid(),
            VoucherName = request.VoucherName,
            MaxDiscount = request.MaxDiscount,
            VoucherCondition = request.VoucherCondition,
            Status = "Active",
            StartDate = request.StartDate,
            ExpiredDate = request.ExpiredDate,
            Description = request.Description
        });

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"User {userId} create voucher {request.VoucherName}",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
