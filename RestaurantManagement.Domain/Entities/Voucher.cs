namespace RestaurantManagement.Domain.Entities;

public class Voucher
{
    public Ulid VoucherId { get; set; }
    public string VoucherName { get; set; }
    public decimal MaxDiscount { get; set; }
    public decimal VoucherCondition { get; set; }
    public string Description { get; set; }
    public ICollection<CustomerVoucher>? CustomerVouchers { get; set; }
    public ICollection<Bill>? Bills { get; set; }
}
