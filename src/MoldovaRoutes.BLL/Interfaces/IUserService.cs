using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.BLL.Interfaces;

/// <summary>
/// Интерфейс сервиса пользователей.
/// Отвечает за регистрацию и поиск пользователей.
/// </summary>
public interface IUserService
{
    /// <summary>Регистрация нового пользователя. Хэширует пароль внутри.</summary>
    Task<UserDto> RegisterUserAsync(CreateUserDto dto);

    /// <summary>Получить пользователя по email. Возвращает null, если не найден.</summary>
    Task<UserDto?> GetUserByEmailAsync(string email);
}
