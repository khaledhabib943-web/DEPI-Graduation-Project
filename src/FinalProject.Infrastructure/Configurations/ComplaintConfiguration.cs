using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            // Table
            builder.ToTable("Complaints");

            // Primary Key
            builder.HasKey(c => c.ComplaintId);

            // Properties with Fluent API Validation
            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.AdminResponse)
                .HasMaxLength(2000);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.ResolvedAt)
                .IsRequired(false);

            builder.Property(c => c.RequestId)
                .IsRequired(false);

            // Relationships
            builder.HasOne(c => c.Request)
                .WithMany()
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.SetNull);

            // Customer & Worker relationships are configured in their respective configs

            // Indexes
            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Complaints_Status");
        }
    }
}
