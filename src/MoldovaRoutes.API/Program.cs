using Microsoft.EntityFrameworkCore;
using MoldovaRoutes.BLL.Interfaces;
using MoldovaRoutes.BLL.Services;
using MoldovaRoutes.Common.Mapping;
using MoldovaRoutes.DAL.Context;

var builder = WebApplication.CreateBuilder(args);

// РЕГИСТРАЦИЯ СЕРВИСОВ (до builder.Build())

// 1. База данных: EF Core + MSSQL
// Строка подключения берётся из appsettings.json (секция ConnectionStrings.Default)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        // Указываем, в каком проекте лежит DbContext (для миграций)
        b => b.MigrationsAssembly("MoldovaRoutes.API")
    )
);

// 2. AutoMapper
// Сканирует сборку и находит все классы, унаследованные от Profile
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 3. Dependency Injection: регистрация сервисов BLL
// AddScoped = один экземпляр на HTTP-запрос (оптимально для сервисов с DbContext)
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ITicketService, TicketService>();

// 4. Контроллеры
builder.Services.AddControllers();

// 5. Swagger / OpenAPI (для тестирования API без Postman)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Moldova Routes API",
        Version = "v1",
        Description = "API для поиска и покупки билетов на маршруты в Молдове. " +
                      "Для доступа к Admin-методам добавьте заголовок X-User-Role: Admin"
    });
});

// 6. CORS: разрешаем запросы с фронтенда (React/Vite на порту 5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// КОНВЕЙЕР ЗАПРОСОВ (Pipeline) — порядок важен!

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Swagger UI доступен только в Development-режиме
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Moldova Routes API v1");
        options.RoutePrefix = string.Empty; // Swagger на корне: http://localhost:5000/
    });
}

app.UseCors("FrontendPolicy"); // Применяем CORS политику

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Автоматическое применение миграций при запуске
// Удобно для разработки. В продакшене лучше запускать миграции вручную.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
