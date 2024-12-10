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
    //Sửa lỗi chưa lấy orderdetail
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
            .ThenInclude(a => a.OrderTransaction)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return Result.Failure(new[] { new Error("Order", "Table does not have any order.") });
        }

        //Kiểm tra transaction
        if (order.OrderTransaction == null)
        {
            return Result.Failure(new[] { new Error("Order", "Transaction not found!.") });
        }

        //Cập nhật transaction  
        order.OrderTransaction.Status = "Paid";
        //Cập nhật trạng thái bàn
        // await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");
        var table = await context.Tables.FindAsync(int.Parse(request.tableId));
        table.ActiveStatus = "Empty";
        //Cập nhật trạng thái order
        order.PaymentStatus = "Paid";

        //Kiểm tra bàn có booking hay không
        var checkBooking = await context.Tables
            .Include(a => a.BookingDetails)
            .ThenInclude(a => a.Booking)
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
        CustomerVoucher? customerVoucher = new CustomerVoucher();
        bool isVoucherUsed = false;
        if (!string.IsNullOrEmpty(order.OrderTransaction.VoucherId.ToString()))
        {
            customerVoucher = await context.CustomerVouchers
                .Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .Where(a => a.VoucherId == order.OrderTransaction.VoucherId && a.Customer.User.Phone == order.OrderTransaction.PayerName)
                .FirstOrDefaultAsync();
            customerVoucher.Quantity -= 1;
            isVoucherUsed = true;
        }

        if (isBooking == true) //Có booking thì cập nhật bill vì khi thanh toán booking đã có bill
        {
            checkBooking.Booking.Bill.Total += order.OrderTransaction.Amount;
            checkBooking.Booking.Bill.IsVoucherUsed = isVoucherUsed;
            checkBooking.Booking.Bill.VoucherId = order.OrderTransaction.VoucherId;
            checkBooking.Booking.Bill.PaymentStatus = "Paid";
            checkBooking.Booking.BookingStatus = "Completed";
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
                Total = order.OrderTransaction.Amount,
                PaymentStatus = "Paid",
                PaymentType = "Cash",
                VoucherId = order.OrderTransaction.VoucherId,
                IsVoucherUsed = isVoucherUsed
            };

            await context.Bills.AddAsync(bill);

        }

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}  

#region Stable code 
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
//             .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
//             .Include(a => a.Orders)
//             .ThenInclude(a => a.OrderTransaction)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

//         if (order == null)
//         {
//             return Result.Failure(new[] { new Error("Order", "Table does not have any order.") });
//         }

//         //Kiểm tra transaction
//         if (order.OrderTransaction == null)
//         {
//             return Result.Failure(new[] { new Error("Order", "Transaction not found!.") });
//         }

//         //Cập nhật transaction  
//         order.OrderTransaction.Status = "Paid";
//         //Cập nhật trạng thái bàn
//         // await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");
//         var table = await context.Tables.FindAsync(int.Parse(request.tableId));
//         table.ActiveStatus = "Empty";
//         //Cập nhật trạng thái order
//         order.PaymentStatus = "Paid";

//         //Kiểm tra bàn có booking hay không
//         var checkBooking = await context.Tables
//             .Include(a => a.BookingDetails)
//             .ThenInclude(a => a.Booking)
//             .ThenInclude(a => a.Bill)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
//             .FirstOrDefaultAsync();

//         bool isBooking = false;
//         if (checkBooking != null)
//         {
//             checkBooking.Booking.BookingStatus = "Completed";
//             isBooking = true;
//         }

//         //Kiểm tra xem khách hàng có sử dụng voucher hay không
//         // CustomerVoucher? customerVoucher = new CustomerVoucher();
//         bool isVoucherUsed = false;
//         if (!string.IsNullOrEmpty(order.OrderTransaction.VoucherId.ToString()))
//         {
//             //TODO: Cập nhật số lượng voucher cho khách hàng

//             // customerVoucher = await context.CustomerVouchers
//             //     .Include(a => a.Customer)
//             //     .ThenInclude(a => a.User)
//             //     .Where(a => a.VoucherId == order.OrderTransaction.VoucherId && a.Customer.User.Phone == order.OrderTransaction.PayerName)
//             //     .FirstOrDefaultAsync();
//             // customerVoucher.Quantity -= 1;
//             isVoucherUsed = true;

//         }

//         if (isBooking == true) //Có booking thì cập nhật bill vì khi thanh toán booking đã có bill
//         {
//             checkBooking.Booking.Bill.Total += order.OrderTransaction.Amount;
//             checkBooking.Booking.Bill.IsVoucherUsed = isVoucherUsed;
//             checkBooking.Booking.Bill.VoucherId = order.OrderTransaction.VoucherId;
//             checkBooking.Booking.Bill.PaymentStatus = "Paid";
//             checkBooking.Booking.BookingStatus = "Completed";
//         }
//         else
//         {
//             //Tạo bill
//             var bill = new Bill
//             {
//                 BillId = Ulid.NewUlid(),
//                 BookId = null,
//                 CreatedDate = DateTime.Now,
//                 OrderId = order.OrderId,
//                 Total = order.OrderTransaction.Amount,
//                 PaymentStatus = "Paid",
//                 PaymentType = "Cash",
//                 VoucherId = order.OrderTransaction.VoucherId,
//                 IsVoucherUsed = isVoucherUsed
//             };

//             await context.Bills.AddAsync(bill);

//         }

//         await unitOfWork.SaveChangesAsync();

//         return Result.Success();
//     }
// }  
#endregion

#region New Stable code
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
//             .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
//             .Include(a => a.Orders)
//             .ThenInclude(a => a.OrderTransaction)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

//         if (order == null)
//         {
//             return Result.Failure(new[] { new Error("Order", "Table does not have any order.") });
//         }

//         //Kiểm tra transaction
//         if (order.OrderTransaction == null)
//         {
//             return Result.Failure(new[] { new Error("Order", "Transaction not found!.") });
//         }

//         //Cập nhật transaction  
//         order.OrderTransaction.Status = "Paid";
//         //Cập nhật trạng thái bàn
//         // await tableRepository.UpdateActiveStatus(int.Parse(request.tableId), "Empty");
//         var table = await context.Tables.FindAsync(int.Parse(request.tableId));
//         table.ActiveStatus = "Empty";
//         //Cập nhật trạng thái order
//         order.PaymentStatus = "Paid";

//         //Kiểm tra bàn có booking hay không
//         var checkBooking = await context.Tables
//             .Include(a => a.BookingDetails)
//             .ThenInclude(a => a.Booking)
//             .ThenInclude(a => a.Bill)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
//             .FirstOrDefaultAsync();

//         bool isBooking = false;
//         if (checkBooking != null)
//         {
//             checkBooking.Booking.BookingStatus = "Completed";
//             isBooking = true;
//         }

//         //Kiểm tra xem khách hàng có sử dụng voucher hay không
//         CustomerVoucher? customerVoucher = new CustomerVoucher();
//         bool isVoucherUsed = false;
//         if (!string.IsNullOrEmpty(order.OrderTransaction.VoucherId.ToString()))
//         {
//             // TODO: Cập nhật số lượng voucher cho khách hàng

//             //Xác định khách hàng sử dụng voucher
//             var customerInfo = await context.Customers.AsNoTracking().Include(a => a.User).Where(a => a.User.Phone == order.OrderTransaction.PayerName).FirstOrDefaultAsync();
//             customerVoucher = await context.CustomerVouchers.Where(a => a.VoucherId == order.OrderTransaction.VoucherId && a.CustomerId == customerInfo.CustomerId).FirstOrDefaultAsync();
//             CustomerVoucher newCustomerVoucher = customerVoucher;
//             newCustomerVoucher.Quantity -= 1;
//             context.CustomerVouchers.Remove(customerVoucher);
//             await unitOfWork.SaveChangesAsync();
//             await context.CustomerVouchers.AddAsync(newCustomerVoucher);
//             isVoucherUsed = true;
//         }

//         if (isBooking == true) //Có booking thì cập nhật bill vì khi thanh toán booking đã có bill
//         {
//             checkBooking.Booking.Bill.Total += order.OrderTransaction.Amount;
//             checkBooking.Booking.Bill.IsVoucherUsed = isVoucherUsed;
//             checkBooking.Booking.Bill.VoucherId = order.OrderTransaction.VoucherId;
//             checkBooking.Booking.Bill.PaymentStatus = "Paid";
//             checkBooking.Booking.BookingStatus = "Completed";
//         }
//         else
//         {
//             //Tạo bill
//             var bill = new Bill
//             {
//                 BillId = Ulid.NewUlid(),
//                 BookId = null,
//                 CreatedDate = DateTime.Now,
//                 OrderId = order.OrderId,
//                 Total = order.OrderTransaction.Amount,
//                 PaymentStatus = "Paid",
//                 PaymentType = "Cash",
//                 VoucherId = order.OrderTransaction.VoucherId,
//                 IsVoucherUsed = isVoucherUsed
//             };

//             await context.Bills.AddAsync(bill);

//         }

//         try
//         {
//             await unitOfWork.SaveChangesAsync();
//         }
//         catch (Exception ex)
//         {
//             return Result.Failure(new[] { new Error("Order", ex.Message) });
//         }


//         return Result.Success();
//     }
// }
#endregion
