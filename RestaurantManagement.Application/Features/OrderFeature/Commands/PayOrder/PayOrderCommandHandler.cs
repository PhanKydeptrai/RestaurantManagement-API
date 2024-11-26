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
            .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
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
        await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");
        //Cập nhật trạng thái order
        order.PaymentStatus = "Paid";

        //Kiểm tra bàn có booking hay không
        var checkBooking = await context.Tables
            .Include(a => a.BookingDetails)
            .Include(a => a.BookingDetails).ThenInclude(a => a.Booking)
            .ThenInclude(a => a.Bill)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();

        bool isBooking = false;
        if (checkBooking != null)
        {
            checkBooking.Booking.BookingStatus = "Completed";
            isBooking = true;
        }

        //Kiểm tra xem khách hàng có sử dụng voucher hay không
        var customerVoucher = new CustomerVoucher();
        bool isVoucherUsed = false;
        if (!string.IsNullOrEmpty(order.OrderTransactions.FirstOrDefault().VoucherId.ToString()))
        {
            customerVoucher = await context.CustomerVouchers
                .Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .Where(a => a.VoucherId == order.OrderTransactions.FirstOrDefault().VoucherId && a.Customer.User.Phone == order.OrderTransactions.FirstOrDefault().PayerName)
                .FirstOrDefaultAsync();
            isVoucherUsed = true;
            customerVoucher.Quantity -= 1;
        }

        if (isBooking == true)
        {
            checkBooking.Booking.Bill.Total += order.OrderTransactions.FirstOrDefault(a => a.OrderId == order.OrderId).Amount;
            await unitOfWork.SaveChangesAsync();
        }
        else
        {
            //Tạo bill
            var bill = new Bill
            {
                BillId = Ulid.NewUlid(),
                BookId = null,
                CreatedDate = DateTime.Now,
                OrderId = order.OrderId,
                Total = order.OrderTransactions.FirstOrDefault(a => a.OrderId == order.OrderId).Amount,
                PaymentStatus = "Paid",
                PaymentType = "Cash",
                IsVoucherUsed = isVoucherUsed
            };

            await context.Bills.AddAsync(bill);
            await unitOfWork.SaveChangesAsync();
        }

        
        return Result.Success();
    }
}

#region Old payorder
// using Microsoft.EntityFrameworkCore;
// using RestaurantManagement.Application.Abtractions;
// using RestaurantManagement.Application.Data;
// using RestaurantManagement.Application.Extentions;
// using RestaurantManagement.Domain.Entities;
// using RestaurantManagement.Domain.IRepos;
// using RestaurantManagement.Domain.Shared;

// namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrder;


// public class PayOrderCommandHandler(
//     IApplicationDbContext context,
//     IUnitOfWork unitOfWork,
//     ITableRepository tableRepository) : ICommandHandler<PayOrderCommand>
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
//             .ThenInclude(a => a.OrderTransactions.Where(a => a.Status == "Unpaid"))
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

//         if (order == null)
//         {
//             var error = new[] { new Error("Order", "Table does not have any order.") };
//             return Result.Failure(error);
//         }

//         //Cập nhật transaction
//         order.OrderTransactions.FirstOrDefault().Status = "Paid";
//         //Cập nhật trạng thái bàn
//         await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");
//         //Cập nhật trạng thái order
//         order.PaymentStatus = "Paid";
//         //Kiểm tra booking bằng transaction
//         try //Có booking thì cập nhật bill vì khi thanh toán booking đã có bill
//         {

//             Booking? booking = await context.Bookings
//                 .Include(a => a.Bill)
//                 .FirstOrDefaultAsync(a => a.Bill.BillId == order.OrderTransactions.FirstOrDefault().BillId && a.BookingStatus == "Occupied");


//             booking.BookingStatus = "Completed";
//             booking.Bill.Total += order.OrderTransactions.FirstOrDefault().Amount;
//             bool isVoucherUsed = false;
//             //Kiêm tra xem có sử dụng voucher hay không
//             if (!string.IsNullOrEmpty(order.OrderTransactions.FirstOrDefault().VoucherId.ToString()))
//             {
//                 var customerVoucher = await context.CustomerVouchers
//                      .Include(a => a.Customer)
//                      .ThenInclude(a => a.User)
//                      .Where(a => a.VoucherId == order.OrderTransactions.FirstOrDefault().VoucherId && a.Customer.User.Phone == order.OrderTransactions.FirstOrDefault().PayerName)
//                      .FirstOrDefaultAsync();

//                 customerVoucher.Quantity -= 1;
//                 isVoucherUsed = true;
//             }
//             booking.Bill.IsVoucherUsed = isVoucherUsed;
//             booking.Bill.PaymentStatus = "Paid";
//             await unitOfWork.SaveChangesAsync();

//         }
//         catch (Exception)
//         {
//             bool isVoucherUsed = false;
//             //Kiểm tra xem có sử dụng voucher hay không
//             if (!string.IsNullOrEmpty(order.OrderTransactions.FirstOrDefault().VoucherId.ToString()))
//             {
//                 var customerVoucher = await context.CustomerVouchers
//                      .Include(a => a.Customer)
//                      .ThenInclude(a => a.User)
//                      .Where(a => a.VoucherId == order.OrderTransactions.FirstOrDefault().VoucherId && a.Customer.User.Phone == order.OrderTransactions.FirstOrDefault().PayerName)
//                      .FirstOrDefaultAsync();

//                 customerVoucher.Quantity -= 1;
//                 isVoucherUsed = true;
//             }

//             //Tạo bill
//             var bill = new Bill
//             {
//                 BillId = Ulid.NewUlid(),
//                 BookId = null,
//                 CreatedDate = DateTime.Now,
//                 OrderId = order.OrderId,
//                 Total = order.OrderTransactions.FirstOrDefault(a => a.OrderId == order.OrderId).Amount,
//                 PaymentStatus = "Paid",
//                 PaymentType = "Cash",
//                 IsVoucherUsed = isVoucherUsed
//             };

//             await context.Bills.AddAsync(bill);

//             await unitOfWork.SaveChangesAsync();
//         }
//         return Result.Success();
//     }
// }
#endregion