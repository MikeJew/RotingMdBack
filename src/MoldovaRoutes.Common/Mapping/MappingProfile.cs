using AutoMapper;
using MoldovaRoutes.Common.DTOs;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.Common.Mapping;

/// <summary>
/// Профиль маппинга AutoMapper.
/// Определяет правила преобразования между Entities (слой DAL) и DTOs (слой Common).
/// AutoMapper автоматически сопоставляет свойства с одинаковыми именами.
/// Метод CreateMap<Source, Destination>() настраивает каждую пару.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ─── Route Mappings ──────────────────────────────────────────────────
        
        // Entity -> DTO (для GET-запросов, возврат клиенту)
        CreateMap<Route, RouteDto>();

        // CreateRouteDto -> Entity (для POST, создание нового маршрута)
        CreateMap<CreateRouteDto, Route>();

        // UpdateRouteDto -> Entity (для PUT)
        // Используем ForAllMembers + Condition, чтобы не перетирать поля,
        // которые не были переданы (значение null в UpdateRouteDto = "не обновлять").
        CreateMap<UpdateRouteDto, Route>()
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null
            ));

        // ─── User Mappings ───────────────────────────────────────────────────

        // Entity -> DTO (БЕЗ PasswordHash — он никогда не уходит клиенту)
        CreateMap<User, UserDto>();

        // CreateUserDto -> Entity
        // PasswordHash маппим вручную в BLL после хэширования пароля.
        // Здесь игнорируем, чтобы AutoMapper не пытался сопоставить Password -> PasswordHash.
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // ─── Ticket Mappings ─────────────────────────────────────────────────

        // Entity -> DTO (с вложенными User и Route DTO — AutoMapper сделает это рекурсивно)
        CreateMap<Ticket, TicketDto>();

        // CreateTicketDto -> Entity
        CreateMap<CreateTicketDto, Ticket>();
    }
}
