namespace RestaurantManagement.Domain.DTOs.PaymentDtos;

public class VnPayReturnModel
{
    public string vnp_Amount { get; set; }
    public string vnp_BankCode { get; set; }
    public string vnp_OrderInfo { get; set; }
    public string vnp_ResponseCode { get; set; }
    public string vnp_TmnCode { get; set; }
    public string vnp_TransactionNo { get; set; }
    public string vnp_TxnRef { get; set; }
    public string vnp_SecureHash { get; set; }

    public static bool TryParse(string input, IFormatProvider formatProvider, out VnPayReturnModel model)
    {
        model = null;
        try
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(input);
            model = new VnPayReturnModel
            {
                vnp_Amount = queryParams["vnp_Amount"],
                vnp_BankCode = queryParams["vnp_BankCode"],
                vnp_OrderInfo = queryParams["vnp_OrderInfo"],
                vnp_ResponseCode = queryParams["vnp_ResponseCode"],
                vnp_TmnCode = queryParams["vnp_TmnCode"],
                vnp_TransactionNo = queryParams["vnp_TransactionNo"],
                vnp_TxnRef = queryParams["vnp_TxnRef"],
                vnp_SecureHash = queryParams["vnp_SecureHash"]
            };
            return true;
        }
        catch
        {
            return false;
        }
    }
}
