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
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVoucherCommand>
{
    public async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {

        //Validate request
        var validator = new CreateVoucherCommandValidator(voucherRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        var voucher = new Voucher
        {
            VoucherId = Ulid.NewUlid(),
            VoucherName = request.VoucherName,
            VoucherCode = request.VoucherCode,
            VoucherType = string.IsNullOrEmpty(request.PercentageDiscount) ? "DirectDiscount" : "PercentageDiscount",
            PercentageDiscount = string.IsNullOrEmpty(request.PercentageDiscount) ? null : int.Parse(request.PercentageDiscount),
            MaximumDiscountAmount = decimal.Parse(request.MaximumDiscountAmount),
            MinimumOrderAmount = decimal.Parse(request.MinimumOrderAmount),
            VoucherConditions = string.IsNullOrEmpty(request.VoucherConditions) ? 0 : decimal.Parse(request.VoucherConditions),
            StartDate = request.StartDate,
            ExpiredDate = request.ExpiredDate,
            Description = request.Description,
            Status = "Active"
        };
        await context.Vouchers.AddAsync(voucher);

        #region Decode jwt and system log
        //decode token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await context.VoucherLogs.AddAsync(new VoucherLog
        {
            VoucherLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Tạo voucher {request.VoucherName}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

#region Old Code
// public class CreateVoucherCommandHanler(
// IApplicationDbContext context,
// IVoucherRepository voucherRepository,
// IUnitOfWork unitOfWork) : ICommandHandler<CreateVoucherCommand>
// {
//     public async Task<Result> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
//     {

//         //Validate request
//         var validator = new CreateVoucherCommandValidator(voucherRepository);
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }




//         //create voucher
//         await context.Vouchers.AddAsync(new Voucher
//         {
//             VoucherId = Ulid.NewUlid(),
//             VoucherName = request.VoucherName,
//             MaxDiscount = request.MaxDiscount,
//             VoucherCondition = request.VoucherCondition,
//             Status = "Active",
//             StartDate = request.StartDate,
//             ExpiredDate = request.ExpiredDate,
//             Description = request.Description
//         });

//         //TODO: Cập nhật system log
//         #region Decode jwt and system log
//         // //Decode jwt
//         // var claims = JwtHelper.DecodeJwt(request.token);
//         // claims.TryGetValue("sub", out var userId);

//         // //Create System Log
//         // await systemLogRepository.CreateSystemLog(new SystemLog
//         // {
//         //     SystemLogId = Ulid.NewUlid(),
//         //     LogDate = DateTime.Now,
//         //     LogDetail = $"User {userId} create voucher {request.VoucherName}",
//         //     UserId = Ulid.Parse(userId)
//         // });
//         #endregion

//         await unitOfWork.SaveChangesAsync();
//         return Result.Success();
//     }
// }
#endregion