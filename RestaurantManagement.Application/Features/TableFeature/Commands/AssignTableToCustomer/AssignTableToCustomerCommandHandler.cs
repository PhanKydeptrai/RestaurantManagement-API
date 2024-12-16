using Microsoft.EntityFrameworkCore;
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
        
        var bookingInfomation = await context.Bookings.Include(a => a.BookingDetails)
            .Where(
                a => a.BookingStatus == "Seated"
                &&
                a.BookingDetails.FirstOrDefault().TableId == int.Parse(request.id)) //đã xếp bàn
            .ToListAsync();


        foreach (var info in bookingInfomation)
        {
            //So sánh ngày book hiện tại với ngaỳ book của booking đã xếp bàn
            //Nếu cùng ngày thì kiểm tra giờ
            if (info.BookingDate.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy"))
            {
                var afterBooking = info.BookingTime.AddHours(+4);
                var beforeBooking = info.BookingTime.AddHours(-2);
                
                if (TimeOnly.FromDateTime(DateTime.Now) >= beforeBooking && TimeOnly.FromDateTime(DateTime.Now) <= afterBooking)
                {
                    return Result.Failure(new[] { new Error("Table", "Table is not available") });
                }
            }
        }

        await tableRepository.UpdateActiveStatus(int.Parse(request.id), "Occupied");

        #region Decode jwt and system log
        //decode token
        var claims = JwtHelper.DecodeJwt(request.token);
        claims.TryGetValue("sub", out var userId);
        var userInfo = await context.Users.FindAsync(Ulid.Parse(userId));
        //Create System Log
        await context.BookingLogs.AddAsync(new BookingLog
        {
            BookingLogId = Ulid.NewUlid(),
            LogDate = DateTime.Now,
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} cho khách nhận bàn {request.id}",
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