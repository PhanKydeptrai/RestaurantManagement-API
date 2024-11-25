namespace RestaurantManagement.Domain.Entities;

public class OrderTransaction
{
    public Ulid TransactionId { get; set; }  // Mã giao dịch
    public string PayerName { get; set; }  // Tên người thanh toán
    public string PayerEmail { get; set; }  // Email người thanh toán
    public decimal Amount { get; set; }  // Số tiền
    public string? Description { get; set; }  // Mô tả giao dịch
    public string Status { get; set; }  // Trạng thái giao dịch
    public DateTime TransactionDate { get; set; }  // Thời gian giao dịch
    public Ulid OrderId { get; set; }
    public Order? Order { get; set; }
}
