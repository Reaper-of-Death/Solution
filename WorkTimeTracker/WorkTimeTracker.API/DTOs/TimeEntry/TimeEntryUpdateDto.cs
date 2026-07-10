using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.DTOs.TimeEntry;

/// <summary>
/// DTO для обновления проводки
/// </summary>
public class TimeEntryUpdateDto
{
    /// <summary>
    /// Дата списания времени
    /// </summary>
    /// <example>2026-07-08</example>
    [Required(ErrorMessage = "Дата обязательна")]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Количество часов
    /// </summary>
    /// <example>4.5</example>
    [Required(ErrorMessage = "Количество часов обязательно")]
    [Range(0.01, 24, ErrorMessage = "Количество часов должно быть от 0.01 до 24")]
    public decimal Hours { get; set; }
    
    /// <summary>
    /// Описание выполненной работы
    /// </summary>
    /// <example>Обновление API для учета времени</example>
    [MaxLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string? Description { get; set; }
}