using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.DeleteVoucher;

public class DeleteVoucherCommandHandler(
    IApplicationDbContext context,
    IVoucherRepository voucherRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteVoucherCommand>
{
    public async Task<Result> Handle(DeleteVoucherCommand request, CancellationToken cancellationToken)
    {
        
        //Validate request
        var validator = new DeleteVoucherCommandValidator(voucherRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Delete Voucher
        await voucherRepository.DeleteVoucher(Ulid.Parse(request.id));
        
        //TODO: Cập nhật system log
        #region Decode jwt and system log
        //Decode jwt
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        var voucherInfo = await context.Vouchers.FindAsync(Ulid.Parse(request.id));
        await context.VoucherLogs.AddAsync(new VoucherLog
        {
            VoucherLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} xoá voucher {voucherInfo.VoucherName}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
