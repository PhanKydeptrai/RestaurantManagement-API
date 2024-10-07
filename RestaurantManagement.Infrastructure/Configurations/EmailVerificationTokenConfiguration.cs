using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Converter;

namespace RestaurantManagement.Infrastructure.Configurations;

internal sealed class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(a => a.EmailVerificationTokenId);
        builder.Property(a => a.EmailVerificationTokenId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.UserId).IsRequired().HasConversion<UlidToStringConverter>();
        builder.Property(a => a.CreatedDate).IsRequired().HasColumnType("datetime");
        builder.Property(a => a.ExpiredDate).IsRequired().HasColumnType("datetime");

        //ForeignKey
        //Một user có nhiều emailverificationtoken
        builder.HasOne(a => a.User).WithMany(a => a.EmailVerificationTokens).HasForeignKey(a => a.UserId);
    }
}
