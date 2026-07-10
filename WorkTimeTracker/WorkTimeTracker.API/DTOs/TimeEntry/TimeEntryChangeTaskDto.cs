using System.ComponentModel.DataAnnotations;

namespace WorkTimeTracker.API.DTOs.TimeEntry;

/// <summary>
/// DTO для смены задачи в проводке
/// </summary>
public class TimeEntryChangeTaskDto
{
    /// <summary>
    /// Новый идентификатор задачи
    /// </summary>
    [Required(ErrorMessage = "Задача обязательна")]
    public Guid TaskId { get; set; }
}