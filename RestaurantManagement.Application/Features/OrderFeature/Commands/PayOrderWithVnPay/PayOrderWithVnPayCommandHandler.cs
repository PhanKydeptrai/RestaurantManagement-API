using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Abtractions;
using RestaurantManagement.Application.Data;
using RestaurantManagement.Application.Extentions;
using RestaurantManagement.Domain.IRepos;
using RestaurantManagement.Domain.Shared;

namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrderWithVnPay;

public class PayOrderWithVnPayCommandHandler : ICommandHandler<PayOrderWithVnPayCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITableRepository _tableRepository;

    public PayOrderWithVnPayCommandHandler(
        IApplicationDbContext context,
        ITableRepository tableRepository,
        IConfiguration configuration)
    {
        _context = context;
        _tableRepository = tableRepository;
        _configuration = configuration;
    }

    public async Task<Result<string>> Handle(PayOrderWithVnPayCommand request, CancellationToken cancellationToken)
    {
        //validate
        var validator = new PayOrderWithVnPayCommandValidator(_tableRepository);
        Error[]? errors = null;
        var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
        if (!isValid)
        {
            return Result<string>.Failure(errors!);
        }
        
        //Kiểm tra xem bàn đã có order chưa
        var order = await _context.Tables
            .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
            .Include(a => a.Orders)
            .ThenInclude(a => a.OrderTransaction)
            .Where(a => a.TableId == int.Parse(request.tableId))
            .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
            .FirstOrDefaultAsync();

        
        if (order == null)
        {
            var error = new[] { new Error("Order", "Table does not have any order.") };
            return Result<string>.Failure(error);
        }

        if(order.OrderTransaction == null)
        {
            var error = new[] { new Error("Order", "Transaction not found!.") };
            return Result<string>.Failure(error);
        }        
        
        //NOTE: VNPAY return URL
        #region VnPay
        string paymentUrl = VnPayExtentions.GetVnPayUrl(
            _configuration["VNP_RETURNURL_ORDERS"]!,
            (int)order.OrderTransaction.Amount,
            order.OrderTransaction.TransactionId.ToString(),
            _configuration);
        #endregion

        return Result<string>.Success(paymentUrl);
    }
}


#region Stable Code
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using RestaurantManagement.Application.Abtractions;
// using RestaurantManagement.Application.Data;
// using RestaurantManagement.Application.Extentions;
// using RestaurantManagement.Application.Services;
// using RestaurantManagement.Domain.IRepos;
// using RestaurantManagement.Domain.Shared;

// namespace RestaurantManagement.Application.Features.OrderFeature.Commands.PayOrderWithVnPay;

// public class PayOrderWithVnPayCommandHandler : ICommandHandler<PayOrderWithVnPayCommand, string>
// {
//     private readonly IApplicationDbContext _context;
//     private readonly IConfiguration _configuration;
//     private readonly ITableRepository _tableRepository;

//     public PayOrderWithVnPayCommandHandler(
//         IApplicationDbContext context,
//         ITableRepository tableRepository,
//         IConfiguration configuration)
//     {
//         _context = context;
//         _tableRepository = tableRepository;
//         _configuration = configuration;
//     }

//     public async Task<Result<string>> Handle(PayOrderWithVnPayCommand request, CancellationToken cancellationToken)
//     {
//         //validate
//         var validator = new PayOrderWithVnPayCommandValidator(_tableRepository);
//         Error[]? errors = null;
//         var isValid = await Task.Run(() => ValidateRequest.RequestValidator(validator, request, out errors));
//         if (!isValid)
//         {
//             return Result<string>.Failure(errors!);
//         }
        
//         //Kiểm tra xem bàn đã có order chưa
//         var order = await _context.Tables
//             .Include(a => a.BookingDetails.Where(a => a.Booking.BookingStatus == "Occupied"))
//             .Include(a => a.Orders)
//             .ThenInclude(a => a.OrderTransaction)
//             .Where(a => a.TableId == int.Parse(request.tableId))
//             .Select(a => a.Orders.FirstOrDefault(a => a.PaymentStatus == "Unpaid"))
//             .FirstOrDefaultAsync();

        
//         if (order == null)
//         {
//             var error = new[] { new Error("Order", "Table does not have any order.") };
//             return Result<string>.Failure(error);
//         }

//         if(order.OrderTransaction == null)
//         {
//             var error = new[] { new Error("Order", "Transaction not found!.") };
//             return Result<string>.Failure(error);
//         }        
        
//         //NOTE: VNPAY return URL
//         #region VnPay
//         //Get Config Info
//         string vnp_Returnurl = _configuration["VNP_RETURNURL"]!; //URL nhan ket qua tra ve 
//         string vnp_Url = _configuration["VNP_URL"]!; //URL thanh toan cua VNPAY 
//         string vnp_TmnCode = _configuration["VNP_TMNCODE"]!; //Ma định danh merchant kết nối (Terminal Id)
//         string vnp_HashSecret = _configuration["VNP_TMNCODE"]!; //Secret Key

//         //Build URL for VNPAY
//         VnPayLibrary vnpay = new VnPayLibrary();

//         vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
//         vnpay.AddRequestData("vnp_Command", "pay");
//         vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
//         vnpay.AddRequestData("vnp_Amount", ((int)order.OrderTransaction.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
//         vnpay.AddRequestData("vnp_BankCode", "");
//         vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
//         vnpay.AddRequestData("vnp_CurrCode", "VND");
//         vnpay.AddRequestData("vnp_IpAddr", ":1");
//         vnpay.AddRequestData("vnp_Locale", "vn");
//         vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderTransaction.TransactionId);
//         vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

//         vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
//         vnpay.AddRequestData("vnp_TxnRef", order.OrderTransaction.TransactionId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

//         string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
//         #endregion

//         return Result<string>.Success(paymentUrl);
//     }
// }

#endregion
