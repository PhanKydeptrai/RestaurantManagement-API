using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class BookingLogConfiguration : IEntityTypeConfiguration<BookingLog>
{
    public void Configure(EntityTypeBuilder<BookingLog> builder)
    {
        builder.HasKey(a => a.BookingLogId);
        builder.Property(a => a.BookingLogId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.LogDetails).IsRequired().HasColumnType("nvarchar(255)");

        //ForeignKey
        builder.HasOne(a => a.User).WithMany(a => a.BookingLogs).HasForeignKey(a => a.UserId);
    }


}
