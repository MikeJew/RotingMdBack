namespace MoldovaRoutes.DAL.Entities;

/// <summary>
/// Сущность купленного билета.
/// Связывает пользователя с маршрутом и фиксирует дату покупки.
/// </summary>
public class Ticket
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int RouteId { get; set; }

    /// <summary>Дата и время совершения покупки (UTC)</summary>
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Route Route { get; set; } = null!;
}
