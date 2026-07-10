namespace WorkTimeTracker.API.DTOs.Task;

/// <summary>
/// DTO для отображения задачи
/// </summary>
public class TaskResponseDto
{
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название задачи
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Название проекта
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;
    
    /// <summary>
    /// Код проекта
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Активна ли задача
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}