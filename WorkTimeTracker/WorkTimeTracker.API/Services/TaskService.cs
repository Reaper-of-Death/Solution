using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Data;
using WorkTimeTracker.API.Models;
using WorkTimeTracker.API.Services.Interfaces;

namespace WorkTimeTracker.API.Services;

/// <summary>
/// Сервис для работы с задачами
/// </summary>
public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WorkTask?> GetTaskByIdAsync(Guid taskId)  
    {
        return await _context.WorkTasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<bool> IsTaskActiveAsync(Guid taskId)
    {
        var task = await _context.WorkTasks 
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        return task != null && task.IsActive;
    }

    public async Task<List<WorkTask>> GetActiveTasksAsync()  
    {
        return await _context.WorkTasks 
            .Include(t => t.Project)
            .Where(t => t.IsActive && t.Project.IsActive)
            .OrderBy(t => t.Project.Code)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<WorkTask>> GetTasksByProjectAsync(Guid projectId)  
    {
        return await _context.WorkTasks 
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<bool> HasTimeEntriesAsync(Guid taskId)
    {
        var count = await _context.TimeEntries
            .CountAsync(te => te.TaskId == taskId);
        
        return count > 0;
    }
}