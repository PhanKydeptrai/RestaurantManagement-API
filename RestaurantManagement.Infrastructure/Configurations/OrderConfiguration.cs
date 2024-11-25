using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(a => a.OrderId);
        builder.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.PaymentStatus).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.Total).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.CustomerId).IsRequired(false).HasConversion<UlidToStringConverter>();
        // builder.Property(a => a.TableId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.TableId).IsRequired().HasColumnType("int");

        builder.Property(a => a.OrderTime).IsRequired().HasColumnType("datetime");
        
        builder.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");

        //ForeignKey
        //Một order có nhiều orderdetail
        builder.HasMany(a => a.OrderDetails).WithOne(a => a.Order).HasForeignKey(a => a.OrderId);
        //Một Customer có nhiều order
        builder.HasOne(a => a.Customer).WithMany(a => a.Orders).HasForeignKey(a => a.CustomerId).IsRequired(false);
        //Một Order có nhiều PaymentTransaction
        builder.HasMany(a => a.PaymentTransactions).WithOne(a => a.Order).HasForeignKey(a => a.OrderId);
    }
}