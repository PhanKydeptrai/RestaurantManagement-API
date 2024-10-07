using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(a => a.EmployeeId);
        builder.Property(a => a.EmployeeId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.EmployeeStatus).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.Role).IsRequired().HasColumnType("varchar(20)");
    }
}
