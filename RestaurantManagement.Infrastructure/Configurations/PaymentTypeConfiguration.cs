using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class PaymentTypeConfiguration : IEntityTypeConfiguration<PaymentType>
{
    public void Configure(EntityTypeBuilder<PaymentType> builder)
    {
        builder.HasKey(a => a.PaymentTypeId);
        builder.Property(a => a.PaymentTypeId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.Name).IsRequired().HasColumnType("varchar(50)");
    }
}
