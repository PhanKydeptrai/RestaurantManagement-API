using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<OrderTransaction>
{
    public void Configure(EntityTypeBuilder<OrderTransaction> builder)
    {
        builder.HasKey(a => a.TransactionId);
        builder.Property(a => a.TransactionId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.PayerName).IsRequired().HasColumnType("nvarchar(255)");
        builder.Property(a => a.PayerEmail).IsRequired().HasColumnType("nvarchar(255)");
        builder.Property(a => a.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
        builder.Property(a => a.Status).IsRequired().HasColumnType("nvarchar(20)");
    }

    // public Ulid TransactionId { get; set; }  // Mã giao dịch
    // public string PayerName { get; set; }  // Tên người thanh toán
    // public string PayerEmail { get; set; }  // Email người thanh toán
    // public string PayeeName { get; set; }  // Tên người nhận
    // public decimal Amount { get; set; }  // Số tiền
    // public string? Description { get; set; }  // Mô tả giao dịch
    // public string Status { get; set; }  // Trạng thái giao dịch
    // public DateTime TransactionDate { get; set; }  // Thời gian giao dịch
    // public Ulid OrderId { get; set; }
    // public Order? Order { get; set; }
}
