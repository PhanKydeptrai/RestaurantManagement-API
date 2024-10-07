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
        builder.Property(a => a.VoucherName).IsRequired().HasColumnType("varchar(50)");
        builder.Property(a => a.MaxDiscount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.VoucherCondition).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
    }
}
