using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.Common.DTOs;
using MoldovaRoutes.DAL.Context;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.BLL.Services;

/// <summary>
/// Реализация сервиса пользователей.
/// Отвечает за регистрацию, хэширование паролей и поиск пользователей.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<UserDto> RegisterUserAsync(CreateUserDto dto)
    {
        // Проверяем, не занят ли Email
        var existingUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (existingUser != null)
        {
            throw new InvalidOperationException($"Пользователь с email {dto.Email} уже существует.");
        }

        // Маппим DTO в Entity
        var user = _mapper.Map<User>(dto);

        // По умолчанию назначаем роль "Client", если не указана
        if (string.IsNullOrWhiteSpace(user.Role))
        {
            user.Role = "Client";
        }

        // Хэшируем пароль (для лабораторной используем SHA256)
        user.PasswordHash = HashPassword(dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// Простая реализация хэширования пароля через SHA256 для учебного проекта.
    /// В реальных проектах следует использовать BCrypt или PasswordHasher.
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
