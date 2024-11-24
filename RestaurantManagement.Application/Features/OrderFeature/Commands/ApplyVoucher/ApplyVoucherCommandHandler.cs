using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.ApplyVoucher;

public class ApplyVoucherCommandHandler : ICommandHandler<ApplyVoucherCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyVoucherCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository,
        IVoucherRepository voucherRepository,
        ITableRepository tableRepository)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _customerRepository = customerRepository;
        _voucherRepository = voucherRepository;
        _tableRepository = tableRepository;
    }

    public async Task<Result> Handle(ApplyVoucherCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new ApplyVoucherCommandValidator(_voucherRepository, _customerRepository, _tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }

        //Apply voucher 
        //Kiểm tra xem bàn đã có order chưa

        var order = await _context.Tables
            .Include(a => a.Orders)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        if (order == null)
        {
            var error = new[] { new Error("Order", "Table does not have any order.") };
            return Result.Failure(error);
        }


        try
        {
            Bill? bill = await _context.Bills // Tìm bill id theo order id
                        .Where(a => a.OrderId == order.OrderId)
                        .FirstOrDefaultAsync();

            bill.CreatedDate = DateTime.Now; //Cập nhật lại thời gian tạo bill

            //lấy voucher theo voucher code
            Voucher voucher = await _context.Vouchers
                .Where(a => a.VoucherCode == request.voucherCode)
                .FirstAsync();

            //Kiểm tra khách hàng có voucher này hay không
            CustomerVoucher? isCustomerHasThisVoucher = await _context.CustomerVouchers
                .Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .Where(a => a.VoucherId == voucher.VoucherId && a.Customer.User.Phone == request.phoneNumber)
                .FirstOrDefaultAsync();

            if (isCustomerHasThisVoucher != null && isCustomerHasThisVoucher.Quantity > 0 /*Khách hàng có voucher*/
                && voucher.MinimumOrderAmount <= bill.Total /*Tổng tiền hóa đơn lớn hơn điều kiện voucher*/)
            {
                bill.Total -= voucher.MaximumDiscountAmount;
                bill.VoucherId = voucher.VoucherId;
                isCustomerHasThisVoucher.Quantity -= 1; //Giảm số lượng voucher của khách hàng
            }
            else
            {
                var error = new[] { new Error("Voucher", "This customer dont have this voucher.") };
                return Result.Failure(error);
            }

        }
        catch (Exception)
        {
            //tạo bill
            var bill = new Bill
            {
                BillId = Ulid.NewUlid(),
                BookId = null,
                CreatedDate = DateTime.Now,
                OrderId = order.OrderId,
                Total = order.Total,
                PaymentStatus = string.Empty,
                PaymentType = string.Empty
            };

            //lấy voucher theo voucher code
            Voucher voucher = await _context.Vouchers
                .Where(a => a.VoucherCode == request.voucherCode)
                .FirstAsync();

            //Kiểm tra khách hàng có voucher này hay không
            CustomerVoucher? isCustomerHasThisVoucher = await _context.CustomerVouchers
                .Include(a => a.Customer)
                .ThenInclude(a => a.User)
                .Where(a => a.VoucherId == voucher.VoucherId && a.Customer.User.Phone == request.phoneNumber)
                .FirstOrDefaultAsync();

            if (isCustomerHasThisVoucher != null && isCustomerHasThisVoucher.Quantity > 0 /*Khách hàng có voucher*/
                && voucher.MinimumOrderAmount <= bill.Total /*Tổng tiền hóa đơn lớn hơn điều kiện voucher*/)
            {
                bill.Total -= voucher.MaximumDiscountAmount;
                bill.VoucherId = voucher.VoucherId;
                isCustomerHasThisVoucher.Quantity -= 1; //Giảm số lượng voucher của khách hàng
            }
            else
            {
                var error = new[] { new Error("Voucher", "This customer dont have this voucher.") };
                return Result.Failure(error);
            }

            await _context.Bills.AddAsync(bill);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}


#region Lmao
// using System.Runtime.CompilerServices;
// using Microsoft.EntityFrameworkCore;
// using RestaurantManagement.Application.Abtractions;
// using RestaurantManagement.Application.Data;
// using RestaurantManagement.Application.Extentions;
// using RestaurantManagement.Domain.Entities;
// using RestaurantManagement.Domain.IRepos;
// using RestaurantManagement.Domain.Shared;

// namespace RestaurantManagement.Application.Features.OrderFeature.Commands.ApplyVoucher;

// public class ApplyVoucherCommandHandler : ICommandHandler<ApplyVoucherCommand>
// {
//     private readonly IApplicationDbContext _context;
//     private readonly ICustomerRepository _customerRepository;
//     private readonly IVoucherRepository _voucherRepository;
//     private readonly ITableRepository _tableRepository;
//     private readonly IUnitOfWork _unitOfWork;

//     public ApplyVoucherCommandHandler(
//         IApplicationDbContext context,
//         IUnitOfWork unitOfWork,
//         ICustomerRepository customerRepository,
//         IVoucherRepository voucherRepository,
//         ITableRepository tableRepository)
//     {
//         _context = context;
//         _unitOfWork = unitOfWork;
//         _customerRepository = customerRepository;
//         _voucherRepository = voucherRepository;
//         _tableRepository = tableRepository;
//     }

//     public async Task<Result> Handle(ApplyVoucherCommand request, CancellationToken cancellationToken)
//     {
//         //validate
//         var validator = new ApplyVoucherCommandValidator(_voucherRepository, _customerRepository, _tableRepository);
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result.Failure(errors!);
//         }

//         //Apply voucher 
//         //Kiểm tra xem bàn đã có order chưa

//         var order = await _context.Tables
//             .Include(a => a.Orders)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

//         if (order == null)
//         {
//             var error = new[] { new Error("Order", "Table does not have any order.") };
//             return Result.Failure(error);
//         }


//         try
//         {
//             Bill? bill = await _context.Bills // Tìm bill id theo order id
//                         .Where(a => a.OrderId == order.OrderId)
//                         .FirstOrDefaultAsync();

//             bill.CreatedDate = DateTime.Now;
//             //Kiểm tra khách có nhập voucher hay không
//             if (!string.IsNullOrEmpty(request.voucherCode))
//             {
//                 //Kiểm tra voucher còn thời hạn hay không?
//                 var voucher = await _context.Vouchers
//                     .Where(a => a.VoucherName == request.voucherCode && a.ExpiredDate >= DateTime.Now)
//                     .FirstOrDefaultAsync();

//                 //Kiểm tra khách hàng có voucher này hay không
//                 var isCustomerHasThisVoucher = await _context.CustomerVouchers
//                     .Include(a => a.Customer)
//                     .ThenInclude(a => a.User)
//                     .Where(a => a.VoucherId == voucher.VoucherId && a.Customer.User.Phone == request.phoneNumber)
//                     .FirstOrDefaultAsync();

//                 if (isCustomerHasThisVoucher != null /*Khách hàng có voucher*/
//                     && voucher != null /*Voucher còn hạn*/
//                     && voucher.MinimumOrderAmount >= bill.Total /*Tổng tiền hóa đơn lớn hơn điều kiện voucher*/)
//                 {
//                     bill.Total -= voucher.MaximumDiscountAmount;
//                     isCustomerHasThisVoucher.Quantity -= 1; //Giảm số lượng voucher của khách hàng
//                 }
//             }
//         }
//         catch (Exception)
//         {
//             //tạo bill
//             var bill = new Bill
//             {
//                 BillId = Ulid.NewUlid(),
//                 BookId = null,
//                 CreatedDate = DateTime.Now,
//                 OrderId = order.OrderId,
//                 Total = order.Total,
//                 PaymentStatus = string.Empty,
//                 PaymentType = string.Empty
//             };

//             //Kiểm tra khách có nhập voucher hay không
//             if (!string.IsNullOrEmpty(request.voucherCode))
//             {
//                 //Kiểm tra voucher còn thời hạn hay không?
//                 Voucher? voucher = await _context.Vouchers
//                     .Where(a => a.VoucherCode == request.voucherCode)
//                     .FirstOrDefaultAsync();

//                 if (voucher == null)
//                 {
//                     var error = new[] { new Error("Order", "Voucher is not found.") };
//                     return Result.Failure(error);
//                 }

//                 //Kiểm tra khách hàng có voucher này hay không
//                 CustomerVoucher? isCustomerHasThisVoucher = await _context.CustomerVouchers
//                     .Include(a => a.Customer)
//                     .ThenInclude(a => a.User)
//                     .Where(a => a.VoucherId == voucher.VoucherId && a.Customer.User.Phone == request.phoneNumber)
//                     .FirstOrDefaultAsync();

//                 if (isCustomerHasThisVoucher != null /*Khách hàng có voucher*/
//                     && voucher != null /*Voucher còn hạn*/
//                     && voucher.MinimumOrderAmount >= bill.Total /*Tổng tiền hóa đơn lớn hơn điều kiện voucher*/)
//                 {
//                     bill.Total -= voucher.MaximumDiscountAmount;
//                     isCustomerHasThisVoucher.Quantity -= 1; //Giảm số lượng voucher của khách hàng
//                 }
//             }

//             await _context.Bills.AddAsync(bill);
//         }

//         await _unitOfWork.SaveChangesAsync();
//         return Result.Success();
//     }
// }

#endregion