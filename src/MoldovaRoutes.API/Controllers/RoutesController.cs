using Microsoft.AspNetCore.Mvc;
using MoldovaRoutes.API.Filters;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.API.Controllers;

/// <summary>
/// Контроллер для управления маршрутами.
/// GET-методы доступны всем (Посетители, Клиенты, Админы).
/// POST, PUT, DELETE защищены атрибутом [AdminMod].
/// Внедряет только IRouteService через конструктор.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>Получить все маршруты (доступно всем)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var routes = await _routeService.GetAllAsync();
        return Ok(routes);
    }

    /// <summary>Получить маршрут по Id (доступно всем)</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var route = await _routeService.GetByIdAsync(id);
        if (route is null)
            return NotFound(new { error = $"Маршрут с Id={id} не найден." });

        return Ok(route);
    }

    /// <summary>
    /// Поиск маршрутов по названию (доступно всем).
    /// Пример: GET /api/routes/search?query=Кишинев
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var routes = await _routeService.SearchAsync(query);
        return Ok(routes);
    }

    /// <summary>
    /// Фильтрация по типу транспорта (доступно всем).
    /// Пример: GET /api/routes/filter?type=bus
    /// </summary>
    [HttpGet("filter")]
    public async Task<IActionResult> FilterByType([FromQuery] string type)
    {
        var routes = await _routeService.FilterByTypeAsync(type);
        return Ok(routes);
    }

    /// <summary>
    /// Создать новый маршрут.
    /// Защищено [AdminMod] — требуется заголовок X-User-Role: Admin.
    /// </summary>
    [HttpPost]
    [AdminMod]
    public async Task<IActionResult> Create([FromBody] CreateRouteDto dto)
    {
        var created = await _routeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновить существующий маршрут.
    /// Защищено [AdminMod] — требуется заголовок X-User-Role: Admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [AdminMod]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto dto)
    {
        try
        {
            var updated = await _routeService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Удалить маршрут.
    /// Защищено [AdminMod] — требуется заголовок X-User-Role: Admin.
    /// </summary>
    [HttpDelete("{id:int}")]
    [AdminMod]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _routeService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
