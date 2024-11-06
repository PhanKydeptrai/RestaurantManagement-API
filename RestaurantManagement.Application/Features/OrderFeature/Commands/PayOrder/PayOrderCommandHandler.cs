using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;

public class PayOrderCommandHandler(
    IApplicationDbContext context,
    IUnitOfWork unitOfWork,
    ITableRepository tableRepository) : ICommandHandler<PayOrderCommand>
{
    public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new PayOrderCommandValidator(tableRepository);
        if (!ValidateRequest.RequestValidator(validator, request, out var errors))
        {
            return Result.Failure(errors);
        }

        //Kiểm tra xem bàn đã có order chưa
       
        var order = await context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == request.tableId)
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();
        
        if(order == null)
        {
            var error = new[] { new Error("Order", "Table does not have any order.") };
            return Result.Failure(error);
        }

        //Thanh toán order
        order.PaymentStatus = "Paid";
        await tableRepository.UpdateActiveStatus(request.tableId, "Empty");

        //Kiểm tra bàn có booking hay không
        var checkBooking = await context.Tables.Include(a => a.BookingDetails)
            .Where(a => a.TableId == request.tableId)
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();

        checkBooking.Booking.BookingStatus = "Completed";
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
