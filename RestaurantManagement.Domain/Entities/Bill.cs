namespace RestaurantManagement.Domain.Entities;

public class Bill
{
    public Ulid BillId { get; set; }
    public string PaymentStatus { get; set; }
    public Ulid OrderId { get; set; }
    public Ulid? BookId { get; set; }
    public Ulid? VoucherId { get; set; }
    public string PaymentType { get; set; }
    public decimal Total { get; set; }
    public Order? Order { get; set; }
    public Booking? Booking { get; set; }
    public Voucher? Voucher { get; set; }
}
