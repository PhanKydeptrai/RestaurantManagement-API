namespace RestaurantManagement.Domain.Entities;

public class Voucher
{
    public Ulid VoucherId { get; set; }
    public string VoucherName { get; set; } 
    public decimal MaxDiscount { get; set; } //Trừ trực tiếp vào hoá đơn
    public decimal VoucherCondition { get; set; } //Điều kiện có thể sử dụng voucher
    public DateTime StartDate { get; set; } //Ngày bắt đầu áp dụng voucher
    public DateTime ExpiredDate { get; set; } //Ngày hết hạn của voucher
    public string? Description { get; set; }
    public string Status { get; set; } //Trạng thái của voucher
    public ICollection<CustomerVoucher>? CustomerVouchers { get; set; }
    public ICollection<Bill>? Bills { get; set; }
}
