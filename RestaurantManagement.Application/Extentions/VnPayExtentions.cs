using Microsoft.Extensions.Configuration;
using RestaurantManagement.Application.Services;

namespace RestaurantManagement.Application.Extentions;

public static class VnPayExtentions
{
    //OrderInfo: Mã đơn hàng, có thể là transactionId hoặc bookingId nếu là thanh toán order hoặc đặt bàn
    public static string GetVnPayUrl(string returnUrl, int amount, string orderInfo, IConfiguration _configuration)
    {
        #region VnPay
        //Get Config Info
        string vnp_Returnurl = returnUrl; //URL nhan ket qua tra ve

        string vnp_Url = _configuration["VNP_URL"]!; //URL thanh toan cua VNPAY 
        string vnp_TmnCode = _configuration["VNP_TMNCODE"]!; //Ma định danh merchant kết nối (Terminal Id)
        string vnp_HashSecret = _configuration["VNP_HASHSECRET"]!; //Secret Key

        //Build URL for VNPAY
        VnPayLibrary vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
        vnpay.AddRequestData("vnp_BankCode", "");
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", ":1");
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderInfo);
        vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", orderInfo); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

        return paymentUrl;
        #endregion
    }
}
