using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(a => a.CustomerId);
        builder.Property(a => a.CustomerId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.CustomerStatus).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.CustomerType).IsRequired().HasColumnType("varchar(20)");

        // ForeignKey
        // Một customer có một user
        builder.HasOne(a => a.User).WithOne(a => a.Customer).HasForeignKey<Customer>(a => a.UserId);
        // Một customer có nhiều customerVoucher
        builder.HasMany(a => a.CustomerVouchers).WithOne(a => a.Customer).HasForeignKey(a => a.CustomerId);
    }
}
