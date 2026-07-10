using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.Models;

/// <summary>
/// Сущность "Задача" в рамках проекта
/// </summary>
public class WorkTask
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Название задачи обязательно")]
    [MaxLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Name { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}