using WorkTimeTracker.API.Models;

namespace WorkTimeTracker.API.Services.Interfaces;

/// <summary>
/// Интерфейс валидатора проводок рабочего времени
/// </summary>
public interface ITimeEntryValidator
{
    /// <summary>
    /// Проверить, не превышает ли общее количество часов за день 24
    /// </summary>
    /// <param name="date">Дата проверки</param>
    /// <param name="newHours">Количество часов в новой проводке</param>
    /// <param name="excludeEntryId">ID проводки, которую исключить из проверки (при обновлении)</param>
    /// <returns>True, если валидация пройдена</returns>
    /// <exception cref="InvalidOperationException">Если сумма часов превышает 24</exception>
    Task ValidateDailyHoursLimitAsync(DateTime date, decimal newHours, Guid? excludeEntryId = null);
    
    /// <summary>
    /// Получить общую сумму часов за день
    /// </summary>
    Task<decimal> GetDailyTotalHoursAsync(DateTime date, Guid? excludeEntryId = null);
    
    /// <summary>
    /// Проверить, можно ли сменить задачу у проводки (задача должна быть активна)
    /// </summary>
    /// <exception cref="InvalidOperationException">Если задача неактивна</exception>
    Task ValidateTaskChangeAsync(Guid taskId);
}