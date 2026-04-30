using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Map Identity's 'Id' PK column to your existing 'UserId' column name
            builder.Property(u => u.Id).HasColumnName("UserId");

            // ── Custom properties (Identity does NOT know about these) ──
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.NationalId)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.Age).IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // NationalId unique index (Identity handles Email & Username uniqueness itself)
            builder.HasIndex(u => u.NationalId)
                .IsUnique()
                .HasDatabaseName("IX_Users_NationalId");

            // Relationship
            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}