namespace WorkTimeTracker.API.DTOs.Project;

/// <summary>
/// DTO для отображения проекта
/// </summary>
public class ProjectResponseDto
{
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название проекта
    /// </summary>
    /// <example>Разработка мобильного приложения</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Код проекта
    /// </summary>
    /// <example>MOB-2026</example>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Активен ли проект
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