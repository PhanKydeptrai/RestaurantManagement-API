using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.UpdateVoucher;

public class UpdateVoucherCommandHandler(
    IVoucherRepository voucherRepository,
    IUnitOfWork unitOfWork,
    ISystemLogRepository systemLogRepository,
    IApplicationDbContext context) : ICommandHandler<UpdateVoucherCommand>
{
    public async Task<Result> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new UpdateVoucherCommandValidator(voucherRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }



        //Update Voucher
        var voucher = await context.Vouchers.FindAsync(request.VoucherId);

        voucher.VoucherName = request.VoucherName;
        voucher.MaxDiscount = request.MaxDiscount;
        voucher.VoucherCondition = request.VoucherCondition;
        voucher.StartDate = request.StartDate;
        voucher.ExpiredDate = request.ExpiredDate;
        voucher.Description = request.Description;


        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);
        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"Update Voucher {request.VoucherId}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

}
