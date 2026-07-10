using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.DTOs.Task;

/// <summary>
/// DTO для создания/обновления задачи
/// </summary>
public class TaskRequestDto
{
    /// <summary>
    /// Название задачи
    /// </summary>
    /// <example>Разработка API для учета времени</example>
    [Required(ErrorMessage = "Название задачи обязательно")]
    [MaxLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Проект обязателен")]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Активна ли задача
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; } = true;
}