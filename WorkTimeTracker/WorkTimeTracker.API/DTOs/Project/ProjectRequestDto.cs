using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.DTOs.Project;

/// <summary>
/// DTO для создания/обновления проекта
/// </summary>
public class ProjectRequestDto
{
    /// <summary>
    /// Название проекта
    /// </summary>
    /// <example>Разработка мобильного приложения</example>
    [Required(ErrorMessage = "Название проекта обязательно")]
    [MaxLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Код проекта
    /// </summary>
    /// <example>MOB-2026</example>
    [Required(ErrorMessage = "Код проекта обязателен")]
    [MaxLength(50, ErrorMessage = "Код не должен превышать 50 символов")]
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Активен ли проект
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; } = true;
}