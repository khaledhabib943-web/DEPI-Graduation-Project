using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            // Table
            builder.ToTable("ServiceRequests");

            // Primary Key
            builder.HasKey(sr => sr.RequestId);

            // Properties with Fluent API Validation
            builder.Property(sr => sr.LocationDetails)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sr => sr.ScheduledDate)
                .IsRequired();

            builder.Property(sr => sr.ScheduledTime)
                .IsRequired();

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(sr => sr.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(sr => sr.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sr => sr.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(sr => sr.Category)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer & Worker relationships are configured in their respective configs

            builder.HasMany(sr => sr.Notifications)
                .WithOne(n => n.RelatedRequest)
                .HasForeignKey(n => n.RelatedRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(sr => sr.Review)
                .WithOne(r => r.Request)
                .HasForeignKey<Review>(r => r.RequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
