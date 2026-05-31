namespace MoldovaRoutes.DAL.Entities;

/// <summary>
/// Сущность маршрута (рейса) общественного транспорта.
/// Управляется только Администратором.
/// </summary>
public class Route
{
    public int Id { get; set; }

    /// <summary>Название направления, например: "Кишинев — Бельцы"</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Тип транспорта: "bus", "train", "minibus"</summary>
    public string TransportType { get; set; } = "bus";

    /// <summary>Название города / станции отправления</summary>
    public string DepartureCity { get; set; } = string.Empty;

    /// <summary>Название города / станции прибытия</summary>
    public string ArrivalCity { get; set; } = string.Empty;

    /// <summary>Время отправления (UTC)</summary>
    public DateTime DepartureTime { get; set; }

    /// <summary>Цена билета в MDL</summary>
    public decimal Price { get; set; }

    /// <summary>Количество свободных мест. Уменьшается при покупке билета.</summary>
    public int AvailableSeats { get; set; }

    /// <summary>Перевозчик, например: "AutoMoldova"</summary>
    public string Company { get; set; } = string.Empty;

    // Навигационное свойство: один маршрут — много купленных билетов
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
