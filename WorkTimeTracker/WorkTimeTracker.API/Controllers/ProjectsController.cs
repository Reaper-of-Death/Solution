using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Data;
using WorkTimeTracker.API.DTOs.Project;
using WorkTimeTracker.API.Models;

namespace WorkTimeTracker.API.Controllers;

/// <summary>
/// Контроллер для управления проектами
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(AppDbContext context, ILogger<ProjectsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить список всех проектов
    /// </summary>
    /// <remarks>
    /// Возвращает все проекты компании, включая неактивные.
    /// </remarks>
    /// <response code="200">Успешное получение списка проектов</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _context.Projects
            .OrderBy(p => p.Code)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return Ok(projects);
    }

    /// <summary>
    /// Получить проект по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор проекта (UUID)</param>
    /// <response code="200">Проект найден</response>
    /// <response code="404">Проект не найден</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound($"Проект с ID {id} не найден");

        return Ok(project);
    }

    /// <summary>
    /// Создать новый проект
    /// </summary>
    /// <param name="dto">Данные для создания проекта</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     POST /api/projects
    ///     {
    ///         "name": "Разработка мобильного приложения",
    ///         "code": "MOB-2026",
    ///         "isActive": true
    ///     }
    /// </remarks>
    /// <response code="201">Проект успешно создан</response>
    /// <response code="400">Некорректные данные или код уже существует</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProjectRequestDto dto)
    {
        // Проверяем уникальность кода
        var existingProject = await _context.Projects
            .AnyAsync(p => p.Code == dto.Code);
            
        if (existingProject)
            return BadRequest($"Проект с кодом '{dto.Code}' уже существует");

        var project = new Project
        {
            Name = dto.Name,
            Code = dto.Code,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var response = new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Code = project.Code,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, response);
    }

    /// <summary>
    /// Обновить проект
    /// </summary>
    /// <param name="id">Идентификатор проекта (UUID)</param>
    /// <param name="dto">Обновленные данные проекта</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     PUT /api/projects/{id}
    ///     {
    ///         "name": "Разработка мобильного приложения v2",
    ///         "code": "MOB-2026-V2",
    ///         "isActive": true
    ///     }
    /// </remarks>
    /// <response code="200">Проект успешно обновлен</response>
    /// <response code="400">Некорректные данные или код уже существует</response>
    /// <response code="404">Проект не найден</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectRequestDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound($"Проект с ID {id} не найден");

        // Проверяем уникальность кода (исключая текущий проект)
        var existingProject = await _context.Projects
            .AnyAsync(p => p.Code == dto.Code && p.Id != id);
            
        if (existingProject)
            return BadRequest($"Проект с кодом '{dto.Code}' уже существует");

        project.Name = dto.Name;
        project.Code = dto.Code;
        project.IsActive = dto.IsActive;
        project.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Code = project.Code,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Удалить проект
    /// </summary>
    /// <param name="id">Идентификатор проекта (UUID)</param>
    /// <remarks>
    /// Проект можно удалить только если у него нет активных задач.
    /// </remarks>
    /// <response code="204">Проект успешно удален</response>
    /// <response code="400">Невозможно удалить проект с активными задачами</response>
    /// <response code="404">Проект не найден</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound($"Проект с ID {id} не найден");

        // Проверяем, есть ли у проекта активные задачи
        if (project.Tasks.Any(t => t.IsActive))
            return BadRequest("Нельзя удалить проект с активными задачами");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}