using Microsoft.AspNetCore.Mvc;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;

namespace MoldovaRoutes.API.Controllers;

/// <summary>
/// Контроллер для регистрации и получения информации о пользователях.
/// Внедряет только IUserService (не DbContext напрямую).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// Принимает имя, email, пароль и (опционально) роль.
    /// Пароль хэшируется в BLL перед сохранением в БД.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await _userService.RegisterUserAsync(dto);
            return StatusCode(StatusCodes.Status201Created, user);
        }
        catch (InvalidOperationException ex)
        {
            // Email уже занят — BLL выбросила ошибку
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Получить пользователя по email.
    /// Пример: GET /api/users?email=test@mail.com
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        return user is null
            ? NotFound(new { error = $"Пользователь с email '{email}' не найден." })
            : Ok(user);
    }
}
