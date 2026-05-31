using Microsoft.AspNetCore.Mvc;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.API.Controllers;

/// <summary>
/// Контроллер для покупки билетов и просмотра истории.
/// Доступ: Клиенты и Администраторы (не Посетители).
/// Для простоты — без [AdminMod], доступен авторизованным пользователям.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    /// <summary>Получить все купленные билеты (для Администратора)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();
        return Ok(tickets);
    }

    /// <summary>
    /// Получить все билеты конкретного пользователя.
    /// Пример: GET /api/tickets/user/3
    /// </summary>
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var tickets = await _ticketService.GetByUserIdAsync(userId);
        return Ok(tickets);
    }

    /// <summary>
    /// Купить билет на маршрут.
    /// Тело запроса: { "userId": 1, "routeId": 2 }
    /// Бизнес-логика проверяет наличие мест и списывает одно место.
    /// </summary>
    [HttpPost("purchase")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Purchase([FromBody] CreateTicketDto dto)
    {
        try
        {
            var ticket = await _ticketService.PurchaseAsync(dto);
            return StatusCode(StatusCodes.Status201Created, ticket);
        }
        catch (KeyNotFoundException ex)
        {
            // Пользователь или маршрут не найден
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Нет свободных мест
            return BadRequest(new { error = ex.Message });
        }
    }
}
