using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.MakePayment;

public class MakePaymentCommandHandler : ICommandHandler<MakePaymentCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITableRepository _tableRepository;
    private readonly IApplicationDbContext _context;
    private readonly IVoucherRepository _voucherRepository;
    private readonly ICustomerRepository _customerRepository;
    public MakePaymentCommandHandler(
        IUnitOfWork unitOfWork,
        ITableRepository tableRepository,
        IApplicationDbContext context,
        IVoucherRepository voucherRepository,
        ICustomerRepository customerRepository)
    {
        _unitOfWork = unitOfWork;
        _tableRepository = tableRepository;
        _context = context;
        _voucherRepository = voucherRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new MakePaymentCommandValidator(
            _tableRepository,
            _voucherRepository,
            _customerRepository
        );

        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result.Failure(errors!);
        }


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

        decimal transactionAmount = order.Total; //Cập nhật giá order cho transaction

        //Kiểm tra bàn có booking hay không
        var checkBooking = await _context.Tables
            .Include(a => a.BookingDetails)
            .Include(a => a.BookingDetails).ThenInclude(a => a.Booking)
            .ThenInclude(a => a.Bill)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.BookingDetails.FirstOrDefault(a => a.Booking.BookingStatus == "Occupied"))
            .FirstOrDefaultAsync();

        Ulid billId = Ulid.Empty;
        if (checkBooking != null) //Tính tiền booking
        {
            transactionAmount = order.Total + (checkBooking.Booking.BookingPrice / 2);
            billId = checkBooking.Booking.Bill.BillId;
        }

        bool isVoucherUsed = false;
        Ulid voucherId = Ulid.Empty;
        if (!string.IsNullOrEmpty(request.voucherCode))
        {
            isVoucherUsed = true;

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
                && voucher.MinimumOrderAmount <= transactionAmount /*Tổng tiền hóa đơn lớn hơn điều kiện voucher*/)
            {
                if (voucher.VoucherType == "DirectDiscount")
                {
                    if (transactionAmount > voucher.MaximumDiscountAmount)
                    {
                        transactionAmount -= voucher.MaximumDiscountAmount;
                    }
                    else
                    {
                        transactionAmount = 0;
                    }
                }
                else
                {
                    decimal discountAmount = transactionAmount * (decimal)voucher.PercentageDiscount / 100;
                    if (discountAmount > voucher.MaximumDiscountAmount)
                    {
                        transactionAmount -= voucher.MaximumDiscountAmount;
                    }
                    else
                    {
                        transactionAmount -= discountAmount;
                    }
                }

                if (billId.ToString() == "00000000000000000000000000")
                {
                    //Tạo transaction
                    var orderTransaction = new OrderTransaction
                    {
                        TransactionId = Ulid.NewUlid(),
                        Status = "Unpaid",
                        Amount = transactionAmount,
                        IsVoucherUsed = isVoucherUsed,
                        TransactionDate = DateTime.Now,
                        PayerEmail = string.Empty,
                        PayerName = request.phoneNumber,
                        OrderId = order.OrderId,
                        BillId = null,
                        PaymentMethod = string.Empty,
                        VoucherId = voucher.VoucherId
                    };

                    await _context.OrderTransactions.AddAsync(orderTransaction);

                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    //Tạo transaction
                    var orderTransaction = new OrderTransaction
                    {
                        TransactionId = Ulid.NewUlid(),
                        Status = "Unpaid",
                        Amount = transactionAmount,
                        IsVoucherUsed = isVoucherUsed,
                        TransactionDate = DateTime.Now,
                        PayerEmail = string.Empty,
                        PayerName = request.phoneNumber,
                        OrderId = order.OrderId,
                        BillId = billId,
                        PaymentMethod = string.Empty,
                        VoucherId = voucher.VoucherId
                    };

                    await _context.OrderTransactions.AddAsync(orderTransaction);

                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
        else
        {
            if (billId.ToString() == "00000000000000000000000000")
            {
                var orderTransaction = new OrderTransaction
                {
                    TransactionId = Ulid.NewUlid(),
                    Status = "Unpaid",
                    Amount = transactionAmount,
                    IsVoucherUsed = isVoucherUsed,
                    TransactionDate = DateTime.Now,
                    PayerEmail = string.Empty,
                    PayerName = string.Empty,
                    OrderId = order.OrderId,
                    PaymentMethod = string.Empty,
                    VoucherId = null,
                    BillId = null
                };

                await _context.OrderTransactions.AddAsync(orderTransaction);

                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                var orderTransaction = new OrderTransaction
                {
                    TransactionId = Ulid.NewUlid(),
                    Status = "Unpaid",
                    Amount = transactionAmount,
                    IsVoucherUsed = isVoucherUsed,
                    TransactionDate = DateTime.Now,
                    PayerEmail = string.Empty,
                    PayerName = string.Empty,
                    OrderId = order.OrderId,
                    PaymentMethod = string.Empty,
                    VoucherId = null,
                    BillId = billId
                };

                await _context.OrderTransactions.AddAsync(orderTransaction);

                await _unitOfWork.SaveChangesAsync();
            }
        }

        return Result.Success();
    }
}
