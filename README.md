# Moldova Routes API — ASP.NET Core Web API

Бэкенд платформы покупки билетов на маршруты Молдовы. Университетская лабораторная работа.

## Архитектура (3 слоя)

```
MoldovaRoutes.sln
├── src/
│   ├── MoldovaRoutes.API/          ← Presentation Layer (Controllers, Filters, Program.cs)
│   ├── MoldovaRoutes.BLL/          ← Business Logic Layer (Interfaces, Services)
│   ├── MoldovaRoutes.DAL/          ← Data Access Layer (Entities, DbContext)
│   └── MoldovaRoutes.Common/       ← Shared (DTOs, AutoMapper MappingProfile)
```

## Технологический стек

| Компонент        | Технология                          |
|------------------|-------------------------------------|
| Фреймворк        | ASP.NET Core 8 Web API              |
| База данных      | MSSQL + Entity Framework Core 8     |
| Маппинг          | AutoMapper 13                       |
| Документация API | Swagger / Swashbuckle               |
| Авторизация      | Кастомный `[AdminMod]` фильтр       |

## Запуск проекта

### 1. Требования
- .NET 8 SDK
- Microsoft SQL Server (локальный или SQL Server Express)
- Visual Studio 2022 / Rider / VS Code

### 2. Настройка подключения к БД
Откройте `src/MoldovaRoutes.API/appsettings.json` и замените строку подключения:
```json
"Default": "Server=YOUR_SERVER;Database=MoldovaRoutesDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Применение миграций EF Core
```bash
cd src/MoldovaRoutes.API
dotnet ef migrations add InitialCreate --project ../MoldovaRoutes.DAL
dotnet ef database update
```

### 4. Запуск
```bash
dotnet run --project src/MoldovaRoutes.API
```
Swagger UI откроется на `http://localhost:5000`

---

## API Эндпоинты

### Routes (Маршруты)

| Метод  | URL                          | Доступ    | Описание                      |
|--------|------------------------------|-----------|-------------------------------|
| GET    | `/api/routes`                | Все       | Все маршруты                  |
| GET    | `/api/routes/{id}`           | Все       | Маршрут по Id                 |
| GET    | `/api/routes/search?query=…` | Все       | Поиск по названию/городу      |
| GET    | `/api/routes/filter?type=…`  | Все       | Фильтр по типу транспорта     |
| POST   | `/api/routes`                | **Admin** | Создать маршрут               |
| PUT    | `/api/routes/{id}`           | **Admin** | Обновить маршрут              |
| DELETE | `/api/routes/{id}`           | **Admin** | Удалить маршрут               |

### Tickets (Билеты)

| Метод  | URL                          | Доступ    | Описание                      |
|--------|------------------------------|-----------|-------------------------------|
| GET    | `/api/tickets`               | Admin     | Все купленные билеты          |
| GET    | `/api/tickets/user/{userId}` | Client    | Билеты конкретного юзера      |
| POST   | `/api/tickets/purchase`      | Client    | Купить билет                  |

---

## Тестирование [AdminMod] в Swagger

Для вызова защищённых эндпоинтов (POST/PUT/DELETE) добавьте заголовок:
```
X-User-Role: Admin
```

Без этого заголовка сервер вернёт `403 Forbidden`.

---

## Пример: Покупка билета (POST /api/tickets/purchase)
```json
{
  "userId": 1,
  "routeId": 2
}
```

Если мест нет → `400 Bad Request` с сообщением об ошибке.
