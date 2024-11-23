namespace RestaurantManagement.Domain.Entities;

#region Old Voucher
// public class Voucher
// {
//     public Ulid VoucherId { get; set; }
//     public string VoucherName { get; set; }
//     public decimal MaxDiscount { get; set; } //Trừ trực tiếp vào hoá đơn
//     public decimal VoucherCondition { get; set; } //Điều kiện có thể sử dụng voucher
//     public DateTime StartDate { get; set; } //Ngày bắt đầu áp dụng voucher
//     public DateTime ExpiredDate { get; set; } //Ngày hết hạn của voucher
//     public string? Description { get; set; }
//     public string Status { get; set; } //Trạng thái của voucher
//     public ICollection<CustomerVoucher>? CustomerVouchers { get; set; }
//     public ICollection<Bill>? Bills { get; set; }
// }
#endregion

#region New Voucher
public class Voucher
{
    public Ulid VoucherId { get; set; }
    public string VoucherName { get; set; } //Tên voucher, Sử dụng để áp dụng
    public string VoucherCode { get; set; } //Sử dụng để áp dụng
    public string VoucherType { get; set; } //Loại voucher 

    // Loại A: Giảm giá trực tiếp vào hóa đơn
    // Loại B: Giảm giá theo phần trăm
    public int? PercentageDiscount { get; set; } //Phần trăm giảm giá
    public decimal MaximumDiscountAmount { get; set; } //Số tiền giảm tối đa
    public decimal MinimumOrderAmount { get; set; } //Khoản tiền tối thiểu để sử dụng voucher
    public decimal VoucherConditions { get; set; } //Điều kiện được nhận voucher dựa theo chi tiêu của khách hàng.
    public DateTime StartDate { get; set; } //Ngày bắt đầu áp dụng voucher
    public DateTime ExpiredDate { get; set; } //Ngày hết hạn của voucher
    public string? Description { get; set; } //Mô tả về voucher
    public string Status { get; set; } //Trạng thái của voucher
    public ICollection<CustomerVoucher>? CustomerVouchers { get; set; }
    public ICollection<Bill>? Bills { get; set; }
}
#endregion


