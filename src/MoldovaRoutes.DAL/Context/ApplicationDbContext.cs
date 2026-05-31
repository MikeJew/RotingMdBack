using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.DAL.Entities;

namespace MoldovaRoutes.DAL.Context;

/// <summary>
/// Главный класс контекста базы данных.
/// Является "мостом" между C# объектами и таблицами MSSQL.
/// EF Core использует этот класс для генерации миграций и выполнения запросов.
/// </summary>
public class ApplicationDbContext : DbContext
{
    // DbSet<T> — представляет таблицу в БД.
    // Через эти свойства пишутся все LINQ-запросы.
    public DbSet<User> Users { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Настройка схемы БД: связи, уникальность полей, значения по умолчанию.
    /// Вызывается EF Core при построении модели (один раз при запуске).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Конфигурация User ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(u => u.Email).IsUnique(); // Email уникален
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Role).IsRequired().HasDefaultValue("Client");
        });

        // --- Конфигурация Route ---
        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Price).HasColumnType("decimal(10,2)");
            entity.Property(r => r.AvailableSeats).HasDefaultValue(0);
        });

        // --- Конфигурация Ticket и его связи ---
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);

            // Связь: Ticket -> User (один пользователь — много билетов)
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tickets)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить юзера с билетами

            // Связь: Ticket -> Route (один маршрут — много билетов)
            entity.HasOne(t => t.Route)
                  .WithMany(r => r.Tickets)
                  .HasForeignKey(t => t.RouteId)
                  .OnDelete(DeleteBehavior.Restrict); // Нельзя удалить маршрут с билетами
        });
    }
}
