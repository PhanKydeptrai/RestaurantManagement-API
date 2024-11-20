using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToCustomer;



#region Stable version
public class GetTableForCustomerCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<AssignTableToCustomerCommand>
{
    public async Task<Result> Handle(AssignTableToCustomerCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new AssignTableToCustomerCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }
        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Occupied");

        await context.Bills.AddAsync(new Bill
        {
            BillId = Ulid.NewUlid(),
            CreatedDate = DateTime.Now,
            Total = 0,
            PaymentStatus = "Unpaid",
            PaymentType = "Cash"
        });



        #region Decode jwt and system log
        //decode token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);

        //Create System Log
        await context.BookingLogs.AddAsync(new BookingLog
        {
            BookingLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"Cho khách nhận bàn {request.id}",
            UserId = Ulid.Parse(userId)
        });
        #endregion

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
#endregion

#region In development
// public class GetTableForCustomerCommandHandler(
//     IApplicationDbContext context,
//     ITableRepository tableRepository,
//     IUnitOfWork unitOfWork) : ICommandHandler<AssignTableToCustomerCommand>
// {
//     public async Task<Result> Handle(AssignTableToCustomerCommand request, CancellationToken cancellationToken)
//     {

//         //Validate request
//         var validator = new AssignTableToCustomerCommandValidator(tableRepository, context);

//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }

//         await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Occupied");
//         await unitOfWork.SaveChangesAsync();
//         return Result.Success();
//     }
// }
#endregion