namespace MoldovaRoutes.Common.DTOs;

// Route DTOs

/// <summary>
/// DTO для возврата данных маршрута клиенту (GET-запросы).
/// Содержит все поля, кроме внутренних служебных.
/// </summary>
public class RouteDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TransportType { get; set; } = string.Empty;
    public string DepartureCity { get; set; } = string.Empty;
    public string ArrivalCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string Company { get; set; } = string.Empty;
}

/// <summary>
/// DTO для создания нового маршрута (POST).
/// Используется только Администратором через [AdminMod].
/// </summary>
public class CreateRouteDto
{
    public string Name { get; set; } = string.Empty;
    public string TransportType { get; set; } = "bus";
    public string DepartureCity { get; set; } = string.Empty;
    public string ArrivalCity { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string Company { get; set; } = string.Empty;
}

/// <summary>
/// DTO для обновления существующего маршрута (PUT).
/// Все поля опциональны — обновляем только то, что передано.
/// </summary>
public class UpdateRouteDto
{
    public string? Name { get; set; }
    public string? TransportType { get; set; }
    public string? DepartureCity { get; set; }
    public string? ArrivalCity { get; set; }
    public DateTime? DepartureTime { get; set; }
    public decimal? Price { get; set; }
    public int? AvailableSeats { get; set; }
    public string? Company { get; set; }
}


// User DTOs

/// <summary>
/// DTO для возврата данных пользователя. БЕЗ хэша пароля!
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// DTO для регистрации нового пользователя.
/// Принимает пароль в открытом виде — BLL сама его хэширует.
/// </summary>
public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Client";
}


// Ticket DTOs

/// <summary>
/// DTO для возврата информации о купленном билете.
/// Включает вложенные DTO пользователя и маршрута для удобства клиента.
/// </summary>
public class TicketDto
{
    public int Id { get; set; }
    public DateTime PurchaseDate { get; set; }
    public UserDto User { get; set; } = null!;
    public RouteDto Route { get; set; } = null!;
}

/// <summary>
/// DTO для запроса на покупку билета.
/// Клиент передаёт только Id пользователя и Id маршрута.
/// </summary>
public class CreateTicketDto
{
    public int UserId { get; set; }
    public int RouteId { get; set; }
}
