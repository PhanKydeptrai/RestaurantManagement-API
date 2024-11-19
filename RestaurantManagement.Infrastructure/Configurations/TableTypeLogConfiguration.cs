using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class TableTypeLogConfiguration : IEntityTypeConfiguration<TableTypeLog>
{
    public void Configure(EntityTypeBuilder<TableTypeLog> builder)
    {
        builder.HasKey(a => a.TableTypeLogId);
        builder.Property(a => a.TableTypeLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.TableTypeLogs).HasForeignKey(a => a.UserId);
    }
}
