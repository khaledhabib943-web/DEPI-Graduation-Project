using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            // TPT: Separate table for Worker
            builder.ToTable("Workers");

            // Properties with Fluent API Validation
            builder.Property(w => w.ProfilePicture)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(w => w.IdFrontImage)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(w => w.IdBackImage)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(w => w.Portfolio)
                .HasMaxLength(500);

            builder.Property(w => w.ServicePrice)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(w => w.AvailabilityStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(w => w.AverageRating)
                .IsRequired()
                .HasDefaultValue(0f);

            builder.Property(w => w.IsValidated)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(w => w.Category)
                .WithMany(c => c.Workers)
                .HasForeignKey(w => w.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(w => w.ServiceRequests)
                .WithOne(sr => sr.Worker)
                .HasForeignKey(sr => sr.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(w => w.Reviews)
                .WithOne(r => r.Worker)
                .HasForeignKey(r => r.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(w => w.Complaints)
                .WithOne(c => c.Worker)
                .HasForeignKey(c => c.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Favorite relationship is auto-mapped via Data Annotations on the Favorite entity
        }
    }
}
