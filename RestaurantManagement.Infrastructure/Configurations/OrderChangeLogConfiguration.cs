// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using RestaurantManagement.Domain.Entities;
// using RestaurantManagement.Infrastructure.Converter;

// namespace RestaurantManagement.Infrastructure.Configurations;

// internal sealed class OrderChangeLogConfiguration : IEntityTypeConfiguration<OrderChangeLog>
// {
//     public void Configure(EntityTypeBuilder<OrderChangeLog> builder)
//     {
//         builder.HasKey(a => a.OrderChangeLogId);
//         builder.Property(a => a.OrderChangeLogId).IsRequired().HasConversion<UlidToStringConverter>();
//         builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
//         builder.Property(a => a.LogMessage).IsRequired().HasColumnType("nvarchar(255)");
//         builder.Property(a => a.Note).IsRequired(false).HasColumnType("nvarchar(255)");
//         builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
//         builder.Property(a => a.OrderId).IsRequired().HasConversion<UlidToStringConverter>();

//         //ForeignKey
//         //Một user có nhiều orderchangelog
//         // builder.HasOne(a => a.User).WithMany(a => a.OrderChangeLogs).HasForeignKey(a => a.UserId);
//     }
// }
