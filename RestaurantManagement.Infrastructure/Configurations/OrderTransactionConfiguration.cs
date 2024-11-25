using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class OrderTransactionConfiguration : IEntityTypeConfiguration<OrderTransaction>
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

}
