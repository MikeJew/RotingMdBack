using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoldovaRoutes.API.Filters;

/// <summary>
/// Кастомный атрибут для защиты административных эндпоинтов.
/// Наследуется от ActionFilterAttribute — это специальный базовый класс
/// в ASP.NET Core, который позволяет перехватывать HTTP-запросы
/// ДО того, как они попадут в метод контроллера.
///
/// Для использования достаточно повесить [AdminMod] над нужным методом.
/// Фильтр проверит заголовок X-User-Role: если там не "Admin",
/// запрос будет отклонён с кодом 403 (Forbidden).
///
/// В реальном проекте роль берётся из JWT-токена (Claims),
/// но для лабораторной мы упрощаем и читаем её из заголовка запроса.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AdminModAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Переопределённый метод OnActionExecuting — вызывается ASP.NET Core
    /// автоматически ПЕРЕД каждым вызовом метода контроллера,
    /// помеченного атрибутом [AdminMod].
    /// Параметр context содержит информацию о текущем HTTP-запросе.
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Пытаемся получить роль из заголовка HTTP-запроса "X-User-Role".
        // В Swagger/Postman клиент должен вручную добавить этот заголовок.
        var userRole = context.HttpContext.Request.Headers["X-User-Role"].FirstOrDefault();

        // Сравниваем значение заголовка с "Admin" (без учёта регистра).
        // Если роль не совпадает или заголовок вообще не передан —
        // блокируем запрос и возвращаем 403 Forbidden.
        if (!string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            // Присваиваем context.Result — это сигнал для ASP.NET Core,
            // что метод контроллера вызывать НЕ нужно.
            // Вместо этого клиенту сразу вернётся этот ответ.
            context.Result = new ObjectResult(new
            {
                error = "Доступ запрещён. Требуются права Администратора.",
                requiredRole = "Admin",
                yourRole = string.IsNullOrEmpty(userRole) ? "не определена" : userRole
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return;
        }

        // Если роль == "Admin", вызываем базовую реализацию —
        // она передаёт управление дальше по конвейеру (следующий фильтр
        // или сам метод контроллера).
        base.OnActionExecuting(context);
    }
}
