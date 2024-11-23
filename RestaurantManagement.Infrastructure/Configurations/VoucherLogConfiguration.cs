using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class VoucherLogConfiguration : IEntityTypeConfiguration<VoucherLog>
{
    public void Configure(EntityTypeBuilder<VoucherLog> builder)
    {
        builder.HasKey(a => a.VoucherLogId);
        builder.Property(a => a.VoucherLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.VoucherLogs).HasForeignKey(a => a.UserId);
    }
}
