using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.Models;

/// <summary>
/// Сущность "Проводка" - списание рабочего времени
/// </summary>
public class TimeEntry
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Дата обязательна")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Количество часов обязательно")]
    [Range(0.01, 24, ErrorMessage = "Количество часов должно быть от 0.01 до 24")]
    public decimal Hours { get; set; }
    
    [MaxLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string? Description { get; set; }
    
    public Guid TaskId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Навигационные свойства
    public virtual WorkTask WorkTask { get; set; } = null!;
}