using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class CategoryLogConfiguration : IEntityTypeConfiguration<CategoryLog>
{
    public void Configure(EntityTypeBuilder<CategoryLog> builder)
    {
        builder.HasKey(a => a.CategoryLogId);
        builder.Property(a => a.CategoryLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.CategoryLogs).HasForeignKey(a => a.UserId);
    }
}
