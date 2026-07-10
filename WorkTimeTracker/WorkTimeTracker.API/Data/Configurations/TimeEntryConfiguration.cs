using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkTimeTracker.API.Models;

namespace WorkTimeTracker.API.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        
        builder.HasKey(te => te.Id);
        
        builder.Property(te => te.Date)
            .IsRequired();
            
        builder.Property(te => te.Hours)
            .IsRequired()
            .HasPrecision(5, 2);
            
        builder.Property(te => te.Description)
            .HasMaxLength(500);
            
        builder.Property(te => te.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(te => te.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.HasOne(te => te.WorkTask)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(te => te.TaskId)
            .HasDatabaseName("IX_TimeEntries_TaskId");
            
        builder.HasIndex(te => te.Date)
            .HasDatabaseName("IX_TimeEntries_Date");
            
        builder.HasIndex(te => new { te.Date, te.TaskId })
            .HasDatabaseName("IX_TimeEntries_Date_TaskId");
    }
}