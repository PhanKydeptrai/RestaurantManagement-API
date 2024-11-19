// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using RestaurantManagement.Domain.Entities;
// using RestaurantManagement.Infrastructure.Converter;

// namespace RestaurantManagement.Infrastructure.Configurations;

// internal sealed class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
// {
//     public void Configure(EntityTypeBuilder<SystemLog> builder)
//     {
//         builder.HasKey(a => a.SystemLogId);
//         builder.Property(a => a.SystemLogId).IsRequired().HasConversion<UlidToStringConverter>();
//         builder.Property(a => a.LogDetail).IsRequired().HasColumnType("nvarchar(255)");
//         builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
//         builder.Property(a => a.LogDate).IsRequired().HasColumnType("datetime");
//     }
// }
