using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.TableFeature.Commands.AssignTableToBookedCustomer;

public class AssignTableToBookedCustomerCommandHandler(
    ITableRepository tableRepository,
    IUnitOfWork unitOfWork,
    IApplicationDbContext context) : ICommandHandler<AssignTableToBookedCustomerCommand>
{
    public async Task<Result> Handle(AssignTableToBookedCustomerCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        var validator = new AssignTableToBookedCustomerCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //lấy thông tin booking

        var booking = await context.Bookings.Include(a => a.BookingDetails)
            .Where(a => a.BookingDetails.Any(b => b.TableId == int.Parse(request.tableId) && a.BookingStatus == "Seated"))
            .FirstOrDefaultAsync();

        booking.BookingStatus = "Occupied";

        await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Occupied");

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
            LogDetails = $"{userInfo.FirstName + " " + userInfo.LastName} cho khách nhận bàn {request.tableId}",
            UserId = Ulid.Parse(userId)
        });
        #endregion
        
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
