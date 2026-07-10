using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Data;
using WorkTimeTracker.API.Services.Interfaces;

namespace WorkTimeTracker.API.Services;

/// <summary>
/// Валидатор проводок рабочего времени
/// </summary>
public class TimeEntryValidator : ITimeEntryValidator
{
    private readonly AppDbContext _context;
    private readonly ILogger<TimeEntryValidator> _logger;

    public TimeEntryValidator(AppDbContext context, ILogger<TimeEntryValidator> logger)
    {
        _context = context;
        _logger = logger;
    }

/// <summary>
/// Проверить лимит часов за день (не более 24)
/// </summary>
/// <summary>
/// Проверить лимит часов за день (не более 24)
/// </summary>
public async Task ValidateDailyHoursLimitAsync(DateTime date, decimal newHours, Guid? excludeEntryId = null)
{
    var dateUtc = date.Date;
    
    var totalHours = await GetDailyTotalHoursAsync(dateUtc, excludeEntryId);
    
    // Добавляем новые часы
    var totalWithNew = totalHours + newHours;
    
    _logger.LogDebug(
        "Проверка лимита часов за день {Date}: текущие {TotalHours} + новые {NewHours} = {TotalWithNew}",
        dateUtc.ToShortDateString(), totalHours, newHours, totalWithNew
    );
    
    if (totalWithNew > 24)
    {
        var errorMessage = $"Превышен лимит часов за день {dateUtc.ToShortDateString()}. " +
                          $"Текущие часы: {totalHours}ч + добавляемые: {newHours}ч = {totalWithNew}ч (максимум 24ч)";
        
        _logger.LogWarning(errorMessage);
        throw new InvalidOperationException(errorMessage);
    }
}

    /// <summary>
    /// Получить общую сумму часов за день
    /// </summary>
    public async Task<decimal> GetDailyTotalHoursAsync(DateTime date, Guid? excludeEntryId = null)
{
    var dateUtc = date.Date;
    
    var query = _context.TimeEntries
        .Where(te => te.Date.Date == dateUtc);
    
    // Исключаем текущую проводку при обновлении
    if (excludeEntryId.HasValue)
    {
        query = query.Where(te => te.Id != excludeEntryId.Value);
    }
    
    var totalHours = await query.SumAsync(te => te.Hours);
    return totalHours;
}

    /// <summary>
    /// Проверить, можно ли сменить задачу (задача должна быть активна)
    /// </summary>
    public async Task ValidateTaskChangeAsync(Guid taskId)
    {
        var task = await _context.WorkTasks  // ИЗМЕНЕНО: Tasks -> WorkTasks
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null)
        {
            throw new InvalidOperationException($"Задача с ID {taskId} не найдена");
        }
        
        if (!task.IsActive)
        {
            throw new InvalidOperationException(
                $"Нельзя выбрать неактивную задачу '{task.Name}'. " +
                "Для изменения задачи выберите активную задачу."
            );
        }
    }
}