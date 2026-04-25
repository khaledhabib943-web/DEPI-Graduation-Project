using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Table
            builder.ToTable("Reviews");

            // Primary Key
            builder.HasKey(r => r.ReviewId);

            // Properties with Fluent API Validation
            builder.Property(r => r.Rating)
                .IsRequired();

            // Check constraint: Rating must be between 1 and 5
            builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            // One review per service request
            builder.HasIndex(r => r.RequestId)
                .IsUnique()
                .HasDatabaseName("IX_Reviews_RequestId");

            // Relationships are configured in Customer, Worker, and ServiceRequest configs
        }
    }
}
