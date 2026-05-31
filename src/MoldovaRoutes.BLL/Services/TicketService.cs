using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;
using MoldovaRoutes.DAL.Context;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.BLL.Services;

/// <summary>
/// Реализация сервиса для работы с билетами.
/// Ключевая бизнес-логика: атомарная покупка билета с проверкой мест.
/// </summary>
public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TicketService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TicketDto>> GetAllAsync()
    {
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.User)    // Eager loading: загружаем связанного пользователя
            .Include(t => t.Route)   // Eager loading: загружаем связанный маршрут
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TicketDto>> GetByUserIdAsync(int userId)
    {
        var tickets = await _context.Tickets
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.Route)
            .Where(t => t.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Бизнес-логика покупки (3 шага):
    /// 1. Найти маршрут по RouteId, убедиться что он существует.
    /// 2. Проверить, что AvailableSeats > 0. Если нет — кинуть исключение.
    /// 3. Атомарно: уменьшить AvailableSeats на 1 и создать новый Ticket.
    ///    Оба действия в одном SaveChangesAsync() — либо оба, либо никакого.
    /// </remarks>
    public async Task<TicketDto> PurchaseAsync(CreateTicketDto dto)
    {
        // Шаг 1: Проверяем существование пользователя
        var user = await _context.Users.FindAsync(dto.UserId)
            ?? throw new KeyNotFoundException($"Пользователь с Id={dto.UserId} не найден.");

        // Шаг 2: Находим маршрут (с отслеживанием, чтобы изменить AvailableSeats)
        var route = await _context.Routes.FindAsync(dto.RouteId)
            ?? throw new KeyNotFoundException($"Маршрут с Id={dto.RouteId} не найден.");

        // Шаг 3: Бизнес-проверка — есть ли свободные места
        if (route.AvailableSeats <= 0)
        {
            throw new InvalidOperationException(
                $"На маршрут '{route.Name}' нет свободных мест.");
        }

        // Шаг 4: Списываем одно место и создаём билет
        route.AvailableSeats--; // Уменьшаем счётчик мест

        var ticket = new Ticket
        {
            UserId = dto.UserId,
            RouteId = dto.RouteId,
            PurchaseDate = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(); // Атомарная операция: и место, и билет сразу

        // Шаг 5: Загружаем навигационные свойства для маппинга в TicketDto
        await _context.Entry(ticket).Reference(t => t.User).LoadAsync();
        await _context.Entry(ticket).Reference(t => t.Route).LoadAsync();

        return _mapper.Map<TicketDto>(ticket);
    }
}
