using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(a => a.TableId);
        builder.Property(a => a.TableId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.TableTypeId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.TableStatus).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.ActiveStatus).IsRequired().HasColumnType("varchar(20)");
        //ForeignKey
        //Một table có nhiều order
        builder.HasMany(a => a.Orders).WithOne(a => a.Table).HasForeignKey(a => a.TableId);
    }
}
