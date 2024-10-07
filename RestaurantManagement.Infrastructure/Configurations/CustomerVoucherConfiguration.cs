using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class CustomerVoucherConfiguration : IEntityTypeConfiguration<CustomerVoucher>
{
    public void Configure(EntityTypeBuilder<CustomerVoucher> builder)
    {
        builder.HasKey(a => a.CustomerVoucherId);
        builder.Property(a => a.CustomerVoucherId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.VoucherId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Quantity).IsRequired().HasColumnType("int");

        // ForeignKey
        // Một voucher có nhiều customerVoucher
        builder.HasOne(a => a.Voucher).WithMany(a => a.CustomerVouchers).HasForeignKey(a => a.VoucherId);
    }
}
