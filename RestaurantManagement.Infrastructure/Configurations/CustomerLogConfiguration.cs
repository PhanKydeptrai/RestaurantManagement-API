using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class CustomerLogConfiguration : IEntityTypeConfiguration<CustomerLog>
{
    public void Configure(EntityTypeBuilder<CustomerLog> builder)
    {
        builder.HasKey(a => a.CustomerLogId);
        builder.Property(a => a.CustomerLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.CustomerLogs).HasForeignKey(a => a.UserId);
    }
}
