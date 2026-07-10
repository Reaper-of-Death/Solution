namespace WorkTimeTracker.API.DTOs.TimeEntry;

/// <summary>
/// DTO для отображения проводки
/// </summary>
public class TimeEntryResponseDto
{
    /// <summary>
    /// Идентификатор проводки
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата списания времени
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Количество часов
    /// </summary>
    public decimal Hours { get; set; }
    
    /// <summary>
    /// Описание выполненной работы
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public Guid TaskId { get; set; }
    
    /// <summary>
    /// Название задачи
    /// </summary>
    public string TaskName { get; set; } = string.Empty;
    
    /// <summary>
    /// Название проекта
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}