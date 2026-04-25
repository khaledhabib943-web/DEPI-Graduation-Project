using FinalProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalProject.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // TPT: Separate table for Customer
            builder.ToTable("Customers");

            // Properties with Fluent API Validation
            builder.Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(300);

            // Relationships
            builder.HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Customer)
                .HasForeignKey(sr => sr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Reviews)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Complaints)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Favorite relationship is auto-mapped via Data Annotations on the Favorite entity
        }
    }
}
