using WorkTimeTracker.API.Models;

namespace WorkTimeTracker.API.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с задачами
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Получить задачу по ID
    /// </summary>
    Task<WorkTask?> GetTaskByIdAsync(Guid taskId);
    
    /// <summary>
    /// Проверить, активна ли задача
    /// </summary>
    Task<bool> IsTaskActiveAsync(Guid taskId);
    
    /// <summary>
    /// Получить все активные задачи
    /// </summary>
    Task<List<WorkTask>> GetActiveTasksAsync();
    
    /// <summary>
    /// Получить задачи по проекту
    /// </summary>
    Task<List<WorkTask>> GetTasksByProjectAsync(Guid projectId);
    
    /// <summary>
    /// Проверить, есть ли у задачи проводки
    /// </summary>
    Task<bool> HasTimeEntriesAsync(Guid taskId);
}