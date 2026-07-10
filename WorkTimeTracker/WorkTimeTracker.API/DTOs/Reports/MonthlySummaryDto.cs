namespace WorkTimeTracker.API.DTOs.Reports;

/// <summary>
/// DTO для месячной сводки
/// </summary>
public class MonthlySummaryDto
{
    /// <summary>
    /// Год
    /// </summary>
    public int Year { get; set; }
    
    /// <summary>
    /// Месяц
    /// </summary>
    public int Month { get; set; }
    
    /// <summary>
    /// Список дней с детализацией
    /// </summary>
    public List<DailySummaryDto> Days { get; set; } = new();
    
    /// <summary>
    /// Общее количество часов за месяц
    /// </summary>
    public decimal MonthTotalHours { get; set; }
}