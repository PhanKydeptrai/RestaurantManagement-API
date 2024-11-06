using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;

public class UpdateVoucherCommandHandler : ICommandHandler<UpdateVoucherCommand>
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IApplicationDbContext _context;
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVoucherCommandHandler(
        IVoucherRepository voucherRepository,
        IUnitOfWork unitOfWork,
        ISystemLogRepository systemLogRepository,
        IApplicationDbContext context)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _systemLogRepository = systemLogRepository;
        _context = context;
    }

    public async Task<Result> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UpdateVoucherCommandValidator(_voucherRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Update Voucher
        var voucher = await _context.Vouchers.FindAsync(request.VoucherId);
        
        voucher.VoucherName = request.VoucherName;
        voucher.MaxDiscount = request.MaxDiscount;
        voucher.VoucherCondition = request.VoucherCondition;
        voucher.StartDate = request.StartDate;
        voucher.ExpiredDate = request.ExpiredDate;
        voucher.Description = request.Description;

        //Create System Log
        await _systemLogRepository.CreateSystemLog(new SystemLog
        {
            SystemLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetail = $"Update Voucher {request.VoucherId}",
            UserId = Ulid.Parse(userId)
        });

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

}
