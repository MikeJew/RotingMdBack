using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;
using MoldovaRoutes.DAL.Context;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.BLL.Services;

/// <summary>
/// Реализация сервиса для работы с маршрутами.
/// Содержит CRUD + поиск/фильтрацию.
/// Получает DbContext и IMapper через Dependency Injection (DI).
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

    /// <inheritdoc/>
    public async Task<IEnumerable<RouteDto>> GetAllAsync()
    {
        var routes = await _context.Routes
            .AsNoTracking() // Только для чтения — лучше производительность
            .ToListAsync();

        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }

    /// <inheritdoc/>
    public async Task<RouteDto?> GetByIdAsync(int id)
    {
        var route = await _context.Routes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return route is null ? null : _mapper.Map<RouteDto>(route);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RouteDto>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllAsync();

        var lowerQuery = query.ToLower();

        // Поиск по подстроке в названии, городе отправления или прибытия
        var routes = await _context.Routes
            .AsNoTracking()
            .Where(r =>
                r.Name.ToLower().Contains(lowerQuery) ||
                r.DepartureCity.ToLower().Contains(lowerQuery) ||
                r.ArrivalCity.ToLower().Contains(lowerQuery))
            .ToListAsync();

        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RouteDto>> FilterByTypeAsync(string transportType)
    {
        var routes = await _context.Routes
            .AsNoTracking()
            .Where(r => r.TransportType.ToLower() == transportType.ToLower())
            .ToListAsync();

        return _mapper.Map<IEnumerable<RouteDto>>(routes);
    }

    /// <inheritdoc/>
    public async Task<RouteDto> CreateAsync(CreateRouteDto dto)
    {
        var route = _mapper.Map<Route>(dto);
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        return _mapper.Map<RouteDto>(route);
    }

    /// <inheritdoc/>
    public async Task<RouteDto?> UpdateAsync(int id, UpdateRouteDto dto)
    {
        // Ищем маршрут с отслеживанием (без AsNoTracking), чтобы EF знал об изменениях
        var route = await _context.Routes.FindAsync(id);
        if (route is null) return null;

        // AutoMapper применяет только непустые поля (настроено в MappingProfile)
        _mapper.Map(dto, route);
        await _context.SaveChangesAsync();
        return _mapper.Map<RouteDto>(route);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var route = await _context.Routes.FindAsync(id);
        if (route is null) return false;

        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
        return true;
    }
}
