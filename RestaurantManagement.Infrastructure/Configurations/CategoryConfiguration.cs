using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(a => a.CategoryId);
        builder.Property(a => a.CategoryId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.CategoryName).IsRequired().HasColumnType("varchar(100)");
        builder.Property(a => a.ImageUrl).IsRequired(false).HasColumnType("varchar(200)");
        builder.Property(a => a.CategoryStatus).IsRequired().HasColumnType("varchar(50)");
    }
}
