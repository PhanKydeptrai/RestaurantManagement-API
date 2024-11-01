using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(a => a.BookId);
        builder.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();

        builder.Property(a => a.BookingTime).IsRequired().HasColumnType("time");
        builder.Property(a => a.BookingDate).IsRequired().HasColumnType("date");
        builder.Property(a => a.BookingPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");

        //ForeignKey
        //Một Customer có nhiều booking
        builder.HasOne(a => a.Customer).WithMany(a => a.Bookings).HasForeignKey(a => a.CustomerId);
        //Một booking có nhiều bookingchangelog
        builder.HasMany(a => a.BookingChangeLogs).WithOne(a => a.Booking).HasForeignKey(a => a.BookId);
        //Một booking có nhiều bookingdetail
        builder.HasMany(a => a.BookingDetails).WithOne(a => a.Booking).HasForeignKey(a => a.BookId);
    }
}
