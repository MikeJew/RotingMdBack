using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.BLL.Interfaces;

/// <summary>
/// Интерфейс сервиса маршрутов.
/// Контроллер зависит ТОЛЬКО от этого интерфейса, а не от конкретной реализации.
/// Это позволяет легко подменить реализацию (например, для тестов).
/// </summary>
public interface IRouteService
{
    /// <summary>Получить все маршруты</summary>
    Task<IEnumerable<RouteDto>> GetAllAsync();

    /// <summary>Получить маршрут по Id. Возвращает null, если не найден.</summary>
    Task<RouteDto?> GetByIdAsync(int id);

    /// <summary>
    /// Поиск маршрутов по подстроке в названии (Name),
    /// городе отправления или прибытия.
    /// </summary>
    Task<IEnumerable<RouteDto>> SearchAsync(string query);

    /// <summary>Фильтрация по типу транспорта (bus / train / minibus)</summary>
    Task<IEnumerable<RouteDto>> FilterByTypeAsync(string transportType);

    /// <summary>Создать новый маршрут (только Админ)</summary>
    Task<RouteDto> CreateAsync(CreateRouteDto dto);

    /// <summary>Обновить маршрут (только Админ)</summary>
    Task<RouteDto?> UpdateAsync(int id, UpdateRouteDto dto);

    /// <summary>Удалить маршрут (только Админ)</summary>
    Task<bool> DeleteAsync(int id);
}
