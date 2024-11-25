using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
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
        //Validate request
        var validator = new PayOrderCommandValidator(tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Kiểm tra xem bàn đã có order chưa
        var order = await context.Tables
            .Include(a => a.Orders)
            .ThenInclude(a => a.OrderTransactions.Where(a => a.Status == "Unpaid"))
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        if (order == null)
        {
            var error = new[] { new Error("Order", "Table does not have any order.") };
            return Result.Failure(error);
        }

        //Cập nhật transaction
        order.OrderTransactions.FirstOrDefault().Status = "Paid";
        //Cập nhật trạng thái bàn
        order.Table.ActiveStatus = "Empty";

        //Kiểm tra booking bằng transaction
        try //Có booking thì cập nhật bill vì khi thanh toán booking đã có bill
        {
            Booking booking = await context.Bookings
                .Include(a => a.Bill)
                .FirstOrDefaultAsync(a => a.Bill.BillId == order.OrderTransactions.FirstOrDefault().BillId);

            booking.BookingStatus = "Completed";
            booking.Bill.Total += order.OrderTransactions.FirstOrDefault().Amount;
            
            await unitOfWork.SaveChangesAsync();

        }
        catch (Exception)
        {
            //Tạo bill
            var bill = new Bill
            {
                BillId = Ulid.NewUlid(),
                BookId = Ulid.Empty,
                CreatedDate = DateTime.Now,
                OrderId = order.OrderId,
                Total = order.Total,
                PaymentStatus = "Paid",
                PaymentType = "Cash"
            };
            await unitOfWork.SaveChangesAsync();
        }
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}

#region Old payorder
// public class PayOrderCommandHandler(
// IApplicationDbContext context,
// IUnitOfWork unitOfWork,
// ITableRepository tableRepository) : ICommandHandler<PayOrderCommand>
// {
//     public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
//     {
//         //Validate request
//         var validator = new PayOrderCommandValidator(tableRepository);
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }
//         //Kiểm tra xem bàn đã có order chưa
//         var order = await context.Tables
//             .Include(a => a.Orders)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

//         if (order == null)
//         {
//             var error = new[] { new Error("Order", "Table does not have any order.") };
//             return Result.Failure(error);
//         }

//         //Thanh toán order
//         order.PaymentStatus = "Paid";
//         await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");

//         //Kiểm tra bàn có booking hay không
//         var checkBooking = await context.Tables
//             .Include(a => a.BookingDetails)
//             .Include(a => a.BookingDetails).ThenInclude(a => a.Booking)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
//             .FirstOrDefaultAsync();

//         if (checkBooking != null)
//         {
//             order.Total = order.Total + (checkBooking.Booking.BookingPrice / 2);
//             checkBooking.Booking.BookingStatus = "Completed";
//         }

//         try
//         {
//             Bill? bill = await context.Bills // Tìm bill id theo order id
//                         .Where(a => a.OrderId == order.OrderId)
//                         .FirstOrDefaultAsync();

//             bill.PaymentStatus = "Paid";
//             bill.PaymentType = "Cash";
//             bill.Total += order.Total;
//             bill.CreatedDate = DateTime.Now;
//         }
//         catch (Exception)
//         {
//             //tạo bill 
//             var bill = new Bill
//             {
//                 BillId = Ulid.NewUlid(),
//                 BookId = checkBooking?.BookId ?? null,
//                 CreatedDate = DateTime.Now,
//                 OrderId = order.OrderId,
//                 Total = order.Total,
//                 PaymentStatus = "Paid",
//                 PaymentType = "Cash"
//             };

//             await context.Bills.AddAsync(bill);
//         }
//         await unitOfWork.SaveChangesAsync();
//         return Result.Success();
//     }
// }
#endregion