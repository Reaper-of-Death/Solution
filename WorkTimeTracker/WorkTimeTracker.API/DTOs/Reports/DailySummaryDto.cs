namespace WorkTimeTracker.API.DTOs.Reports;

/// <summary>
/// DTO для дневной сводки (для визуализации стикеров)
/// </summary>
public class DailySummaryDto
{
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Общее количество часов за день
    /// </summary>
    public decimal TotalHours { get; set; }
    
    /// <summary>
    /// Статус: "under" - меньше 8ч, "exact" - ровно 8ч, "over" - больше 8ч
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Текстовое описание статуса
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Цвет стикера: "yellow", "green", "red"
    /// </summary>
    public string Color { get; set; } = string.Empty;
}