using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTimeTracker.API.Models;

namespace WorkTimeTracker.API.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("IX_Projects_Code_Unique");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Projects_IsActive")
            .HasFilter("\"IsActive\" = TRUE");
    }
}