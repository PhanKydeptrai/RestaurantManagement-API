using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class EmployeeLogConfiguration : IEntityTypeConfiguration<EmployeeLog>
{
    public void Configure(EntityTypeBuilder<EmployeeLog> builder)
    {
        builder.HasKey(a => a.EmployeeLogId);
        builder.Property(a => a.EmployeeLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.EmployeeLogs).HasForeignKey(a => a.UserId);
    }
}
