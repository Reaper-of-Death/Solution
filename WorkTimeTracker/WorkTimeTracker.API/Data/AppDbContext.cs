using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Models;
using WorkTimeTracker.API.Data.Configurations;

namespace WorkTimeTracker.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<WorkTask> WorkTasks { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new WorkTaskConfiguration());
        modelBuilder.ApplyConfiguration(new TimeEntryConfiguration());
    }
    
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Project || e.Entity is WorkTask || e.Entity is TimeEntry)
            .Where(e => e.State == EntityState.Modified);
            
        foreach (var entry in entries)
        {
            var property = entry.Property("UpdatedAt");
            if (property != null)
            {
                property.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}