using Microsoft.AspNetCore.Mvc;
using MoldovaRoutes.API.Filters;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.API.Controllers;

/// <summary>
/// Контроллер для управления маршрутами (рейсами).
/// 
/// Доступ:
///   GET  — доступны всем (Посетители, Клиенты, Администраторы).
///   POST, PUT, DELETE — только Администраторам (защищено [AdminMod]).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    // Зависимость только от интерфейса, а не от конкретного класса.
    // DI-контейнер подставит RouteService при создании контроллера.
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    // ─── GET: доступны всем ─────────────────────────────────────────────────

    /// <summary>Получить все маршруты</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var routes = await _routeService.GetAllAsync();
        return Ok(routes);
    }

    /// <summary>Получить маршрут по Id</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var route = await _routeService.GetByIdAsync(id);
        return route is null ? NotFound(new { message = $"Маршрут с Id={id} не найден." }) : Ok(route);
    }

    /// <summary>
    /// Поиск маршрутов по ключевому слову.
    /// Ищет по названию, городу отправления и прибытия.
    /// Пример: GET /api/routes/search?query=Кишинев
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var routes = await _routeService.SearchAsync(query);
        return Ok(routes);
    }

    /// <summary>
    /// Фильтрация маршрутов по типу транспорта.
    /// Пример: GET /api/routes/filter?type=bus
    /// </summary>
    [HttpGet("filter")]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FilterByType([FromQuery] string type)
    {
        var routes = await _routeService.FilterByTypeAsync(type);
        return Ok(routes);
    }

    // ─── POST, PUT, DELETE: только Администратор ────────────────────────────

    /// <summary>
    /// Создать новый маршрут.
    /// ТРЕБУЕТ роль Admin (заголовок X-User-Role: Admin).
    /// </summary>
    [HttpPost]
    [AdminMod] // ← Наш кастомный фильтр проверяет роль ДО выполнения метода
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateRouteDto dto)
    {
        var created = await _routeService.CreateAsync(dto);
        // Возвращаем 201 Created с URL на созданный ресурс
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Обновить маршрут. ТРЕБУЕТ роль Admin.
    /// </summary>
    [HttpPut("{id:int}")]
    [AdminMod]
    [ProducesResponseType(typeof(RouteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto dto)
    {
        var updated = await _routeService.UpdateAsync(id, dto);
        return updated is null
            ? NotFound(new { message = $"Маршрут с Id={id} не найден." })
            : Ok(updated);
    }

    /// <summary>
    /// Удалить маршрут. ТРЕБУЕТ роль Admin.
    /// </summary>
    [HttpDelete("{id:int}")]
    [AdminMod]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _routeService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(new { message = $"Маршрут с Id={id} не найден." });
    }
}
