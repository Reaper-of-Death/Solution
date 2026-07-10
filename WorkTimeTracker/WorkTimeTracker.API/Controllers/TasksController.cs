using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Data;
using WorkTimeTracker.API.DTOs.Task;
using WorkTimeTracker.API.Models;
using WorkTimeTracker.API.Services.Interfaces;

namespace WorkTimeTracker.API.Controllers;

/// <summary>
/// Контроллер для управления задачами
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(AppDbContext context, ITaskService taskService, ILogger<TasksController> logger)
    {
        _context = context;
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех задач
    /// </summary>
    /// <remarks>
    /// Возвращает все задачи компании, включая неактивные, с информацией о проектах.
    /// </remarks>
    /// <response code="200">Успешное получение списка задач</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _context.WorkTasks  
            .Include(t => t.Project)
            .OrderBy(t => t.Project.Code)
            .ThenBy(t => t.Name)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                ProjectCode = t.Project.Code,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Получить задачу по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор задачи (UUID)</param>
    /// <response code="200">Задача найдена</response>
    /// <response code="404">Задача не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _context.WorkTasks  
            .Include(t => t.Project)
            .Where(t => t.Id == id)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                ProjectCode = t.Project.Code,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (task == null)
            return NotFound($"Задача с ID {id} не найдена");

        return Ok(task);
    }

    /// <summary>
    /// Получить задачи по проекту
    /// </summary>
    /// <param name="projectId">Идентификатор проекта (UUID)</param>
    /// <response code="200">Успешное получение списка задач</response>
    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var tasks = await _context.WorkTasks  
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                ProjectCode = t.Project.Code,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Получить только активные задачи
    /// </summary>
    /// <remarks>
    /// Возвращает задачи, которые активны и принадлежат активным проектам.
    /// Используется для выбора задач при создании проводок.
    /// </remarks>
    /// <response code="200">Успешное получение списка активных задач</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        var tasks = await _context.WorkTasks  
            .Include(t => t.Project)
            .Where(t => t.IsActive && t.Project.IsActive)
            .OrderBy(t => t.Project.Code)
            .ThenBy(t => t.Name)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                ProjectCode = t.Project.Code,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Создать новую задачу
    /// </summary>
    /// <param name="dto">Данные для создания задачи</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     POST /api/tasks
    ///     {
    ///         "name": "Разработка API для учета времени",
    ///         "projectId": "123e4567-e89b-12d3-a456-426614174000",
    ///         "isActive": true
    ///     }
    /// </remarks>
    /// <response code="201">Задача успешно создана</response>
    /// <response code="400">Некорректные данные или задача уже существует</response>
    /// <response code="404">Проект не найден</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] TaskRequestDto dto)
    {
        var project = await _context.Projects.FindAsync(dto.ProjectId);
        if (project == null)
            return NotFound($"Проект с ID {dto.ProjectId} не найден");

        if (!project.IsActive)
            return BadRequest("Нельзя создать задачу для неактивного проекта");

        var existingTask = await _context.WorkTasks  
            .AnyAsync(t => t.Name == dto.Name && t.ProjectId == dto.ProjectId);
            
        if (existingTask)
            return BadRequest($"Задача с названием '{dto.Name}' уже существует в этом проекте");

        var task = new WorkTask  
        {
            Name = dto.Name,
            ProjectId = dto.ProjectId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WorkTasks.Add(task);  
        await _context.SaveChangesAsync();

        await _context.Entry(task).Reference(t => t.Project).LoadAsync();

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            ProjectCode = task.Project.Code,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
    }

    /// <summary>
    /// Обновить задачу
    /// </summary>
    /// <param name="id">Идентификатор задачи (UUID)</param>
    /// <param name="dto">Обновленные данные задачи</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     PUT /api/tasks/{id}
    ///     {
    ///         "name": "Разработка API для учета времени v2",
    ///         "projectId": "123e4567-e89b-12d3-a456-426614174000",
    ///         "isActive": false
    ///     }
    /// </remarks>
    /// <response code="200">Задача успешно обновлена</response>
    /// <response code="400">Некорректные данные или задача уже существует</response>
    /// <response code="404">Задача не найдена</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaskRequestDto dto)
    {
        var task = await _context.WorkTasks  
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            return NotFound($"Задача с ID {id} не найдена");

        var project = await _context.Projects.FindAsync(dto.ProjectId);
        if (project == null)
            return NotFound($"Проект с ID {dto.ProjectId} не найден");

        if (!project.IsActive)
            return BadRequest("Нельзя назначить задачу на неактивный проект");

        var existingTask = await _context.WorkTasks  
            .AnyAsync(t => t.Name == dto.Name && t.ProjectId == dto.ProjectId && t.Id != id);
            
        if (existingTask)
            return BadRequest($"Задача с названием '{dto.Name}' уже существует в этом проекте");

        if (task.IsActive && !dto.IsActive)
        {
            var hasEntries = await _taskService.HasTimeEntriesAsync(id);
            if (hasEntries)
            {
                _logger.LogWarning("Попытка деактивировать задачу {TaskId} с существующими проводками", id);
            }
        }

        task.Name = dto.Name;
        task.ProjectId = dto.ProjectId;
        task.IsActive = dto.IsActive;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Name = task.Name,
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            ProjectCode = task.Project.Code,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Удалить задачу
    /// </summary>
    /// <param name="id">Идентификатор задачи (UUID)</param>
    /// <remarks>
    /// Задачу можно удалить только если на нее нет проводок времени.
    /// </remarks>
    /// <response code="204">Задача успешно удалена</response>
    /// <response code="400">Невозможно удалить задачу с проводками времени</response>
    /// <response code="404">Задача не найдена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _context.WorkTasks.FindAsync(id);  
        if (task == null)
            return NotFound($"Задача с ID {id} не найдена");

        var hasEntries = await _taskService.HasTimeEntriesAsync(id);
        if (hasEntries)
            return BadRequest("Нельзя удалить задачу, так как на нее есть проводки времени");

        _context.WorkTasks.Remove(task);  
        await _context.SaveChangesAsync();

        return NoContent();
    }
}