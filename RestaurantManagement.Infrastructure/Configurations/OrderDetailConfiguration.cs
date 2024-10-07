using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(a => a.OrderDetailId);
        builder.Property(a => a.OrderDetailId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.MealId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Quantity).IsRequired().HasColumnType("int");
        builder.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
        builder.Property(a => a.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");

        //ForeignKey
        //Một meal có nhiều orderdetail
        builder.HasOne(a => a.Meal).WithMany(a => a.OrderDetails).HasForeignKey(a => a.MealId);
    }
}
