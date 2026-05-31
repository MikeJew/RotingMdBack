using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.BLL.Interfaces;

/// <summary>
/// Интерфейс сервиса билетов.
/// Содержит бизнес-логику покупки: проверку мест и атомарное списание.
/// </summary>
public interface ITicketService
{
    /// <summary>Получить все купленные билеты</summary>
    Task<IEnumerable<TicketDto>> GetAllAsync();

    /// <summary>Получить все билеты конкретного пользователя</summary>
    Task<IEnumerable<TicketDto>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Купить билет на маршрут.
    /// Бизнес-логика: проверяет наличие мест и уменьшает AvailableSeats.
    /// Выбрасывает InvalidOperationException, если мест нет.
    /// </summary>
    Task<TicketDto> PurchaseAsync(CreateTicketDto dto);
}
