namespace MoldovaRoutes.DAL.Entities;

/// <summary>
/// Сущность пользователя системы.
/// Хранит данные для аутентификации и определения роли.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Отображаемое имя пользователя</summary>
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Хэш пароля (BCrypt). Никогда не передаём в DTO!</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Роль: "Admin", "Client", "Visitor"</summary>
    public string Role { get; set; } = "Client";

    // Навигационное свойство: один пользователь — много билетов
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
