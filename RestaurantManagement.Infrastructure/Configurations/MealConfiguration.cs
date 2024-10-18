using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.HasKey(a => a.MealId);
        builder.Property(a => a.MealId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.MealName).IsRequired().HasColumnType("nvarchar(100)");
        builder.Property(a => a.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.Image).IsRequired(false).HasColumnType("varbinary(max)");
        builder.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");
        builder.Property(a => a.SellStatus).IsRequired().HasColumnType("varchar(50)");
        builder.Property(a => a.MealStatus).IsRequired().HasColumnType("varchar(50)");
        builder.Property(a => a.CategoryId).IsRequired().HasConversion<UlidToStringConverter>();
    }
}
