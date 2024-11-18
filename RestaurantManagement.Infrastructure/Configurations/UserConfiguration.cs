using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(a => a.UserId);
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.FirstName).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(a => a.LastName).IsRequired().HasColumnType("nvarchar(50)");
        builder.Property(a => a.Password).IsRequired(false).HasColumnType("varchar(64)");
        builder.Property(a => a.Phone).IsRequired(false).HasColumnType("varchar(10)");
        builder.Property(a => a.Status).IsRequired().HasColumnType("varchar(20)");
        builder.Property(a => a.Email).IsRequired(false).HasColumnType("varchar(50)");
        builder.Property(a => a.ImageUrl).IsRequired(false).HasColumnType("varchar(255)");
        builder.Property(a => a.Gender).IsRequired(false).HasColumnType("varchar(10)");

        //ForeignKey
        //Một user có nhiều notification
        builder.HasMany(a => a.Notifications).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        //Một user có nhiều systemlog
        builder.HasMany(a => a.SystemLogs).WithOne(a => a.User).HasForeignKey(a => a.UserId);
        //Một user có nhiều BookingChangeLog
        builder.HasMany(a => a.BookingChangeLogs).WithOne(a => a.User).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.NoAction); ;
        //một user có một employee
        builder.HasOne(a => a.Employee).WithOne(a => a.User).HasForeignKey<Employee>(a => a.UserId);
    }
}
