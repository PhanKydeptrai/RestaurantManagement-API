using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class BookingDetailConfiguration : IEntityTypeConfiguration<BookingDetail>
{
    public void Configure(EntityTypeBuilder<BookingDetail> builder)
    {
        builder.HasKey(a => a.BookingDetailId);
        builder.Property(a => a.BookingDetailId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.TableId).IsRequired().HasColumnType("int");
        // builder.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
        // builder.Property(a => a.Quantity).IsRequired().HasColumnType("int");
        // builder.Property(a => a.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.BookId).IsRequired().HasConversion<UlidToStringConverter>();

        //ForeignKey
        //một table có nhiều bookingdetail
        builder.HasOne(a => a.Table).WithMany(a => a.BookingDetails).HasForeignKey(a => a.TableId);

    }
}
