using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkTimeTracker.API.Data;
using WorkTimeTracker.API.DTOs.TimeEntry;
using WorkTimeTracker.API.DTOs.Reports;
using WorkTimeTracker.API.Models;
using WorkTimeTracker.API.Services.Interfaces;

namespace WorkTimeTracker.API.Controllers;

/// <summary>
/// Контроллер для управления проводками рабочего времени
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class TimeEntriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITimeEntryValidator _validator;
    private readonly ITaskService _taskService;
    private readonly ILogger<TimeEntriesController> _logger;

    public TimeEntriesController(
        AppDbContext context,
        ITimeEntryValidator validator,
        ITaskService taskService,
        ILogger<TimeEntriesController> logger)
    {
        _context = context;
        _validator = validator;
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Получить все проводки с фильтрацией
    /// </summary>
    /// <param name="date">Фильтр по конкретной дате (YYYY-MM-DD)</param>
    /// <param name="month">Фильтр по месяцу (YYYY-MM)</param>
    /// <param name="week">Фильтр по неделе (YYYY-WW)</param>
    /// <remarks>
    /// Примеры фильтрации:
    /// 
    ///     GET /api/timeentries?date=2026-07-08
    ///     GET /api/timeentries?month=2026-07
    ///     GET /api/timeentries?week=2026-28
    /// </remarks>
    /// <response code="200">Успешное получение списка проводок</response>
    /// <response code="400">Неверный формат фильтра</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
    [FromQuery] DateTime? date = null,
    [FromQuery] string? month = null,
    [FromQuery] string? week = null)
    {
        var query = _context.TimeEntries
            .Include(te => te.WorkTask)
            .ThenInclude(t => t.Project)
            .AsQueryable();

        if (date.HasValue)
        {
            var dateUtc = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            query = query.Where(te => te.Date.Date == dateUtc);
        }
        else if (!string.IsNullOrEmpty(month))
        {
            if (DateTime.TryParse(month + "-01", out var monthDate))
            {
                var startOfMonth = DateTime.SpecifyKind(new DateTime(monthDate.Year, monthDate.Month, 1), DateTimeKind.Utc);
                var endOfMonth = DateTime.SpecifyKind(startOfMonth.AddMonths(1).AddDays(-1), DateTimeKind.Utc);
                query = query.Where(te => te.Date.Date >= startOfMonth && te.Date.Date <= endOfMonth);
            }
            else
            {
                return BadRequest("Неверный формат месяца. Используйте YYYY-MM");
            }
        }
        else if (!string.IsNullOrEmpty(week))
        {
            if (TryParseWeek(week, out var startOfWeek, out var endOfWeek))
            {
                var startOfWeekUtc = DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
                var endOfWeekUtc = DateTime.SpecifyKind(endOfWeek, DateTimeKind.Utc);
                query = query.Where(te => te.Date.Date >= startOfWeekUtc && te.Date.Date <= endOfWeekUtc);
            }
            else
            {
                return BadRequest("Неверный формат недели. Используйте YYYY-WW");
            }
        }

        var entries = await query
            .OrderByDescending(te => te.Date)
            .ThenBy(te => te.WorkTask.Project.Code)
            .ThenBy(te => te.WorkTask.Name)
            .Select(te => new TimeEntryResponseDto
            {
                Id = te.Id,
                Date = te.Date,
                Hours = te.Hours,
                Description = te.Description,
                TaskId = te.TaskId,
                TaskName = te.WorkTask.Name,
                ProjectName = te.WorkTask.Project.Name,
                CreatedAt = te.CreatedAt,
                UpdatedAt = te.UpdatedAt
            })
            .ToListAsync();

        return Ok(entries);
    }

    /// <summary>
    /// Получить проводку по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор проводки (UUID)</param>
    /// <response code="200">Проводка найдена</response>
    /// <response code="404">Проводка не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TimeEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entry = await _context.TimeEntries
            .Include(te => te.WorkTask)
            .ThenInclude(t => t.Project)
            .Where(te => te.Id == id)
            .Select(te => new TimeEntryResponseDto
            {
                Id = te.Id,
                Date = te.Date,
                Hours = te.Hours,
                Description = te.Description,
                TaskId = te.TaskId,
                TaskName = te.WorkTask.Name,
                ProjectName = te.WorkTask.Project.Name,
                CreatedAt = te.CreatedAt,
                UpdatedAt = te.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (entry == null)
            return NotFound($"Проводка с ID {id} не найдена");

        return Ok(entry);
    }

    /// <summary>
    /// Создать новую проводку времени
    /// </summary>
    /// <param name="dto">Данные для создания проводки</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     POST /api/timeentries
    ///     {
    ///         "date": "2026-07-08",
    ///         "hours": 4.5,
    ///         "description": "Разработка API для учета времени",
    ///         "taskId": "123e4567-e89b-12d3-a456-426614174000"
    ///     }
    /// 
    /// Ограничения:
    /// - Часы должны быть от 0.01 до 24
    /// - Сумма часов за день не может превышать 24
    /// - Задача должна быть активной
    /// </remarks>
    /// <response code="201">Проводка успешно создана</response>
    /// <response code="400">Некорректные данные или превышен лимит часов</response>
    /// <response code="404">Задача не найдена</response>
    [HttpPost]
    [ProducesResponseType(typeof(TimeEntryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] TimeEntryRequestDto dto)
    {
        var dateUtc = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
        
        var task = await _taskService.GetTaskByIdAsync(dto.TaskId);
        if (task == null)
            return NotFound($"Задача с ID {dto.TaskId} не найдена");

        if (!task.IsActive)
            return BadRequest($"Нельзя создать проводку для неактивной задачи '{task.Name}'");
        
        if (!task.Project.IsActive)
            return BadRequest($"Нельзя создать проводку для задачи из неактивного проекта '{task.Project.Name}'");

        try
        {
            await _validator.ValidateDailyHoursLimitAsync(dateUtc, dto.Hours);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        var entry = new TimeEntry
        {
            Date = dateUtc, 
            Hours = dto.Hours,
            Description = dto.Description,
            TaskId = dto.TaskId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        await _context.Entry(entry).Reference(te => te.WorkTask).LoadAsync();
        await _context.Entry(entry.WorkTask).Reference(t => t.Project).LoadAsync();

        var response = new TimeEntryResponseDto
        {
            Id = entry.Id,
            Date = entry.Date,
            Hours = entry.Hours,
            Description = entry.Description,
            TaskId = entry.TaskId,
            TaskName = entry.WorkTask.Name,
            ProjectName = entry.WorkTask.Project.Name,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, response);
    }

    /// <summary>
    /// Обновить проводку
    /// </summary>
    /// <param name="id">Идентификатор проводки (UUID)</param>
    /// <param name="dto">Обновленные данные проводки</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     PUT /api/timeentries/{id}
    ///     {
    ///         "date": "2026-07-08",
    ///         "hours": 5.0,
    ///         "description": "Обновленное описание работы"
    ///     }
    /// 
    /// Примечание: Поле TaskId нельзя изменить через этот метод.
    /// Для смены задачи используйте PATCH /api/timeentries/{id}/task
    /// </remarks>
    /// <response code="200">Проводка успешно обновлена</response>
    /// <response code="400">Некорректные данные или превышен лимит часов</response>
    /// <response code="404">Проводка не найдена</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TimeEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TimeEntryUpdateDto dto)
    {
        var entry = await _context.TimeEntries
            .Include(te => te.WorkTask)
            .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (entry == null)
            return NotFound($"Проводка с ID {id} не найдена");

        var dateUtc = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);

        try
        {
            await _validator.ValidateDailyHoursLimitAsync(dateUtc, dto.Hours, id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        entry.Date = dateUtc;
        entry.Hours = dto.Hours;
        entry.Description = dto.Description;
        entry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var response = new TimeEntryResponseDto
        {
            Id = entry.Id,
            Date = entry.Date,
            Hours = entry.Hours,
            Description = entry.Description,
            TaskId = entry.TaskId,
            TaskName = entry.WorkTask.Name,
            ProjectName = entry.WorkTask.Project.Name,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Сменить задачу у проводки
    /// </summary>
    /// <param name="id">Идентификатор проводки (UUID)</param>
    /// <param name="dto">Данные с новой задачей</param>
    /// <remarks>
    /// Пример запроса:
    /// 
    ///     PATCH /api/timeentries/{id}/task
    ///     {
    ///         "taskId": "123e4567-e89b-12d3-a456-426614174001"
    ///     }
    /// 
    /// Ограничения:
    /// - Текущая задача должна быть активной
    /// - Новая задача должна быть активной
    /// - Новая задача должна принадлежать активному проекту
    /// </remarks>
    /// <response code="200">Задача успешно изменена</response>
    /// <response code="400">Некорректные данные или задача неактивна</response>
    /// <response code="404">Проводка или задача не найдены</response>
    [HttpPatch("{id}/task")]
    [ProducesResponseType(typeof(TimeEntryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeTask(Guid id, [FromBody] TimeEntryChangeTaskDto dto)
    {
        var entry = await _context.TimeEntries
            .Include(te => te.WorkTask)
            .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (entry == null)
            return NotFound($"Проводка с ID {id} не найдена");

        if (!entry.WorkTask.IsActive)
        {
            return BadRequest(
                $"Нельзя изменить задачу, так как текущая задача '{entry.WorkTask.Name}' неактивна. " +
                "Обратитесь к администратору для изменения."
            );
        }

        var newTask = await _taskService.GetTaskByIdAsync(dto.TaskId);
        if (newTask == null)
            return NotFound($"Задача с ID {dto.TaskId} не найдена");

        if (!newTask.IsActive)
            return BadRequest($"Нельзя выбрать неактивную задачу '{newTask.Name}'");

        if (!newTask.Project.IsActive)
            return BadRequest($"Нельзя выбрать задачу из неактивного проекта '{newTask.Project.Name}'");

        if (entry.TaskId == dto.TaskId)
            return BadRequest("Новая задача совпадает с текущей");

        entry.TaskId = dto.TaskId;
        entry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _context.Entry(entry).Reference(te => te.WorkTask).LoadAsync();
        await _context.Entry(entry.WorkTask).Reference(t => t.Project).LoadAsync();

        var response = new TimeEntryResponseDto
        {
            Id = entry.Id,
            Date = entry.Date,
            Hours = entry.Hours,
            Description = entry.Description,
            TaskId = entry.TaskId,
            TaskName = entry.WorkTask.Name,
            ProjectName = entry.WorkTask.Project.Name,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Удалить проводку
    /// </summary>
    /// <param name="id">Идентификатор проводки (UUID)</param>
    /// <response code="204">Проводка успешно удалена</response>
    /// <response code="404">Проводка не найдена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entry = await _context.TimeEntries.FindAsync(id);
        if (entry == null)
            return NotFound($"Проводка с ID {id} не найдена");

        _context.TimeEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Получить дневную сводку для визуализации стикеров
    /// </summary>
    /// <param name="date">Дата для сводки (YYYY-MM-DD)</param>
    /// <remarks>
    /// Возвращает статус рабочего дня:
    /// - Желтый стикер: менее 8 часов
    /// - Зеленый стикер: ровно 8 часов
    /// - Красный стикер: более 8 часов
    /// </remarks>
    /// <response code="200">Успешное получение сводки</response>
    [HttpGet("summary/daily")]
    [ProducesResponseType(typeof(DailySummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailySummary([FromQuery] DateTime date)
    {
        var dateUtc = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        
        var totalHours = await _validator.GetDailyTotalHoursAsync(dateUtc);
        
        string status;
        string statusMessage;
        string color;
        
        if (totalHours < 8)
        {
            status = "under";
            statusMessage = $"Внесено недостаточно часов: {totalHours:F2}ч (нужно 8ч)";
            color = "yellow";
        }
        else if (totalHours == 8)
        {
            status = "exact";
            statusMessage = $"Внесено ровно 8 часов: {totalHours:F2}ч";
            color = "green";
        }
        else
        {
            status = "over";
            statusMessage = $"Внесено избыточно часов: {totalHours:F2}ч (норма 8ч)";
            color = "red";
        }

        var summary = new DailySummaryDto
        {
            Date = dateUtc,
            TotalHours = totalHours,
            Status = status,
            StatusMessage = statusMessage,
            Color = color
        };

        return Ok(summary);
    }

    /// <summary>
    /// Получить месячную сводку
    /// </summary>
    /// <param name="year">Год (например: 2026)</param>
    /// <param name="month">Месяц (от 1 до 12)</param>
    /// <remarks>
    /// Возвращает детальную сводку по каждому дню месяца с указанием статуса.
    /// </remarks>
    /// <response code="200">Успешное получение сводки</response>
    /// <response code="400">Неверный год или месяц</response>
    [HttpGet("summary/monthly")]
    [ProducesResponseType(typeof(MonthlySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
    {
        if (year < 2000 || year > 2100 || month < 1 || month > 12)
            return BadRequest("Неверный год или месяц");

        // Создаем даты в UTC
        var startOfMonth = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var endOfMonth = DateTime.SpecifyKind(new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59), DateTimeKind.Utc);

        var daysInMonth = DateTime.DaysInMonth(year, month);
        var allDays = Enumerable.Range(1, daysInMonth)
            .Select(day => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc))
            .ToList();

        var entries = await _context.TimeEntries
            .Where(te => te.Date >= startOfMonth && te.Date <= endOfMonth)
            .GroupBy(te => te.Date.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalHours = g.Sum(te => te.Hours)
            })
            .ToDictionaryAsync(x => x.Date, x => x.TotalHours);

        var dailySummaries = allDays.Select(day =>
        {
            var totalHours = entries.ContainsKey(day) ? entries[day] : 0;
            
            string status;
            string statusMessage;
            string color;
            
            if (totalHours < 8)
            {
                status = "under";
                statusMessage = $"Внесено недостаточно: {totalHours:F2}ч (нужно 8ч)";
                color = "yellow";
            }
            else if (totalHours == 8)
            {
                status = "exact";
                statusMessage = "Ровно 8 часов";
                color = "green";
            }
            else
            {
                status = "over";
                statusMessage = $"Внесено избыточно: {totalHours:F2}ч (норма 8ч)";
                color = "red";
            }

            return new DailySummaryDto
            {
                Date = day,
                TotalHours = totalHours,
                Status = status,
                StatusMessage = statusMessage,
                Color = color
            };
        }).ToList();

        var monthTotal = dailySummaries.Sum(d => d.TotalHours);

        var result = new MonthlySummaryDto
        {
            Year = year,
            Month = month,
            Days = dailySummaries,
            MonthTotalHours = monthTotal
        };

        return Ok(result);
    }

    /// <summary>
    /// Разобрать строку недели в даты начала и конца
    /// </summary>
    /// <param name="weekString">Строка в формате YYYY-WW</param>
    /// <param name="startOfWeek">Дата начала недели (понедельник)</param>
    /// <param name="endOfWeek">Дата конца недели (воскресенье)</param>
    /// <returns>True если парсинг успешен</returns>
    private static bool TryParseWeek(string weekString, out DateTime startOfWeek, out DateTime endOfWeek)
    {
        startOfWeek = DateTime.MinValue;
        endOfWeek = DateTime.MinValue;

        try
        {
            var parts = weekString.Split('-');
            if (parts.Length != 2)
                return false;

            var year = int.Parse(parts[0]);
            var week = int.Parse(parts[1]);

            var firstDayOfYear = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - firstDayOfYear.DayOfWeek;
            var firstMonday = firstDayOfYear.AddDays(daysOffset);
            
            if (firstMonday.Year != year)
                firstMonday = firstMonday.AddDays(7);

            startOfWeek = firstMonday.AddDays((week - 1) * 7);
            endOfWeek = startOfWeek.AddDays(6);

            return true;
        }
        catch
        {
            return false;
        }
    }
}