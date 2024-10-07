using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class BookingChangeLogConfiguration : IEntityTypeConfiguration<BookingChangeLog>
{
    public void Configure(EntityTypeBuilder<BookingChangeLog> builder)
    {
        builder.HasKey(a => a.BookingChangeLogId);
        builder.Property(a => a.BookingChangeLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogMessage).IsRequired().HasColumnType("nvarchar(255)");
        builder.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();
    }
}
