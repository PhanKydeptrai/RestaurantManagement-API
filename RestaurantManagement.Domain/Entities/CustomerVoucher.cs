namespace RestaurantManagement.Domain.Entities;

public class CustomerVoucher
{
    public Ulid CustomerVoucherId { get; set; }
    public Ulid VoucherId { get; set; }
    public Ulid CustomerId { get; set; }
    public int Quantity { get; set; }
    public Customer? Customer { get; set; }
    public Voucher? Voucher { get; set; }
}
