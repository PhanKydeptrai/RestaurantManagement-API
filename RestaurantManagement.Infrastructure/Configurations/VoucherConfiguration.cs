using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.HasKey(a => a.VoucherId);
        builder.Property(a => a.VoucherId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.VoucherName).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(a => a.VoucherCode).IsRequired().HasColumnType("varchar(50)");
        builder.Property(a => a.VoucherType).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.PercentageDiscount).IsRequired(false).HasColumnType("int");
        builder.Property(a => a.MaximumDiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.MinimumOrderAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.VoucherConditions).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.StartDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.ExpiredDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.Status).IsRequired().HasColumnType("varchar(50)");
        builder.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
    }
}
