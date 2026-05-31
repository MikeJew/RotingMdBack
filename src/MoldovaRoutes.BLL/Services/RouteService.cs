using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;
using MoldovaRoutes.DAL.Context;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.BLL.Services;

/// <summary>
/// Реализация сервиса для работы с маршрутами.
/// Шаг 3.2: Базовый CRUD (Create, Read, Update, Delete).
/// Получает ApplicationDbContext и IMapper через Dependency Injection (DI).
/// </summary>
public class RouteService : IRouteService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RouteService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // ===================== READ =====================

    /// <inheritdoc/>
    public async Task<IEnumerable<RouteDto>> GetAllAsync()
    {
        // AsNoTracking — отключаем отслеживание изменений,
        // так как данные только для чтения (лучше производительность).
        var routes = await _context.Routes
            .AsNoTracking()
            .ToListAsync();

        // AutoMapper: Entity → DTO (маппинг настроен в MappingProfile)
        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }

    /// <inheritdoc/>
    public async Task<RouteDto?> GetByIdAsync(int id)
    {
        var route = await _context.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        // Если маршрут не найден — возвращаем null, контроллер вернёт 404
        return route is null ? null : _mapper.Map<RouteDto>(route);
    }

    // ===================== SEARCH (заглушка) =====================

    /// <inheritdoc/>
    /// <remarks>
    /// TODO: Шаг 3.3 — реализовать поиск по подстроке в названии маршрута.
    /// Пока метод выбрасывает NotImplementedException.
    /// </remarks>
    public Task<IEnumerable<RouteDto>> SearchAsync(string query)
    {
        // Будет реализовано на следующем шаге (Шаг 3.3)
        throw new NotImplementedException(
            "Метод SearchAsync ещё не реализован. См. Шаг 3.3.");
    }

    /// <inheritdoc/>
    /// <remarks>
    /// TODO: Шаг 3.3 — реализовать фильтрацию по типу транспорта.
    /// </remarks>
    public Task<IEnumerable<RouteDto>> FilterByTypeAsync(string transportType)
    {
        // Будет реализовано на следующем шаге (Шаг 3.3)
        throw new NotImplementedException(
            "Метод FilterByTypeAsync ещё не реализован. См. Шаг 3.3.");
    }

    // ===================== CREATE =====================

    /// <inheritdoc/>
    public async Task<RouteDto> CreateAsync(CreateRouteDto dto)
    {
        // AutoMapper: DTO → Entity (CreateRouteDto → Route)
        var route = _mapper.Map<Route>(dto);

        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        // Возвращаем созданный маршрут как DTO (Entity → RouteDto)
        return _mapper.Map<RouteDto>(route);
    }

    // ===================== UPDATE =====================

    /// <inheritdoc/>
    public async Task<RouteDto?> UpdateAsync(int id, UpdateRouteDto dto)
    {
        // Ищем маршрут БЕЗ AsNoTracking — нужно отслеживание для сохранения изменений
        var route = await _context.Routes.FindAsync(id);

        // Если маршрут не найден — выбрасываем исключение
        if (route is null)
            throw new KeyNotFoundException($"Маршрут с Id={id} не найден в базе данных.");

        // AutoMapper применяет только непустые поля из UpdateRouteDto
        // (настроено через ForAllMembers + Condition в MappingProfile)
        _mapper.Map(dto, route);
        await _context.SaveChangesAsync();

        return _mapper.Map<RouteDto>(route);
    }

    // ===================== DELETE =====================

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var route = await _context.Routes.FindAsync(id);

        // Если маршрут не найден — выбрасываем исключение
        if (route is null)
            throw new KeyNotFoundException($"Маршрут с Id={id} не найден в базе данных.");

        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();

        return true;
    }
}
