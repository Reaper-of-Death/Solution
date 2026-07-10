using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WorkTimeTracker.API.DTOs.TimeEntry;

/// <summary>
/// DTO для создания новой проводки времени
/// </summary>
public class TimeEntryRequestDto
{
    /// <summary>
    /// Дата списания времени (только дата, без времени)
    /// </summary>
    /// <example>2026-07-10</example>
    [Required(ErrorMessage = "Дата обязательна")]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Количество затраченных часов (от 0.01 до 24)
    /// </summary>
    /// <example>4.5</example>
    [Required(ErrorMessage = "Количество часов обязательно")]
    [Range(0.01, 24, ErrorMessage = "Количество часов должно быть от 0.01 до 24")]
    public decimal Hours { get; set; }
    
    /// <summary>
    /// Описание выполненной работы
    /// </summary>
    /// <example>Разработка API для учета времени</example>
    [MaxLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Задача обязательна")]
    public Guid TaskId { get; set; }
}