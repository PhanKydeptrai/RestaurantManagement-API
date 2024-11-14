using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.VoucherFeature.Commands.CreateVoucher;

public class CreateVoucherCommandHanler(
    IApplicationDbContext context,
    IVoucherRepository voucherRepository,
    ISystemLogRepository systemLogRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVoucherCommand>
{
    public async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new CreateVoucherCommandValidator(voucherRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //create voucher
        await context.Vouchers.AddAsync(new Voucher
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

        #region Decode jwt and system log
        // //Decode jwt
        // var claims = JwtHelper.DecodeJwt(request.token);
        // claims.TryGetValue("sub", out var userId);

        // //Create System Log
        // await systemLogRepository.CreateSystemLog(new SystemLog
        // {
        //     SystemLogId = Ulid.NewUlid(),
        //     LogDate = DateTime.Now,
        //     LogDetail = $"User {userId} create voucher {request.VoucherName}",
        //     UserId = Ulid.Parse(userId)
        // });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
