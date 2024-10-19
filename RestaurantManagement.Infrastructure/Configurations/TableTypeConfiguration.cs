using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class TableTypeConfiguration : IEntityTypeConfiguration<TableType>
{
    public void Configure(EntityTypeBuilder<TableType> builder)
    {
        builder.HasKey(a => a.TableTypeId);
        builder.Property(a => a.TableTypeId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.ImageUrl).IsRequired(false).HasColumnType("varchar(250)");
        builder.Property(a => a.TablePrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(a => a.Description).IsRequired(false).HasColumnType("nvarchar(255)");

        //ForeignKey
        //Một tabletype có nhiều table
        builder.HasMany(a => a.Tables).WithOne(a => a.TableType).HasForeignKey(a => a.TableTypeId);
    }
}
