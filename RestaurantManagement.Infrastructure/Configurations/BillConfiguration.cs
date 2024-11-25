using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {

        builder.HasKey(a => a.BillId);
        builder.Property(a => a.BillId).IsRequired().HasConversion<UlidToStringConverter>();

        builder.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");

        builder.Property(a => a.OrderId).IsRequired(false).HasConversion<UlidToStringConverter>();

        builder.Property(a => a.BookId).IsRequired(false).HasConversion<UlidToStringConverter>();

        builder.Property(a => a.VoucherId).IsRequired(false).HasConversion<UlidToStringConverter>();

        builder.Property(a => a.CreatedDate).IsRequired().HasColumnType("datetime");
        
        builder.Property(a => a.PaymentType).IsRequired().HasColumnType("varchar(20)");

        builder.Property(a => a.Total).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.IsVoucherUsed).IsRequired().HasColumnType("bit");

        //ForeignKey
        //Một bill có một order
        builder.HasOne(a => a.Order).WithOne(a => a.Bill).HasForeignKey<Bill>(a => a.OrderId).IsRequired(false);
        //Một bill có một booking?
        builder.HasOne(a => a.Booking).WithOne(a => a.Bill).HasForeignKey<Bill>(a => a.BookId).IsRequired(false);
        //Một voucher có nhiều bill
        builder.HasOne(a => a.Voucher).WithMany(a => a.Bills).HasForeignKey(a => a.VoucherId);
        //Một bill có một transaction
        
        


    }
}
