using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(a => a.NotificationId);
        builder.Property(a => a.NotificationId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Paragraph).IsRequired().HasColumnType("nvarchar(255)");
        builder.Property(a => a.Time).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
    }
}
