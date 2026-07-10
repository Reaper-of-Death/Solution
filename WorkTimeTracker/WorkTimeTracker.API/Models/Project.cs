using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.Models;

/// <summary>
/// Сущность "Проект" компании
/// </summary>
public class Project
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Название проекта обязательно")]
    [MaxLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Код проекта обязателен")]
    [MaxLength(50, ErrorMessage = "Код не должен превышать 50 символов")]
    public string Code { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Навигационное свойство
    public virtual ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();
}