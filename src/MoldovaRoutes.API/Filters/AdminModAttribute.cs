using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoldovaRoutes.API.Filters;

/// <summary>
/// Кастомный атрибут фильтрации для защиты административных эндпоинтов.
/// 
/// КОНЦЕПЦИЯ:
/// ActionFilterAttribute — это перехватчик, который ASP.NET Core вызывает
/// ДО выполнения метода контроллера. Мы используем это, чтобы проверить
/// права пользователя ещё до того, как бизнес-логика запустится.
/// 
/// ИСПОЛЬЗОВАНИЕ:
/// [AdminMod] — просто навешиваем этот атрибут над нужным методом контроллера.
/// 
/// ПРИНЦИП РАБОТЫ:
/// 1. Пользователь делает HTTP-запрос к эндпоинту с [AdminMod].
/// 2. ASP.NET Core видит атрибут и вызывает OnActionExecuting().
/// 3. Мы читаем роль из HTTP-заголовка "X-User-Role" (эмуляция авторизации).
/// 4. Если роль != "Admin" — немедленно возвращаем 403 Forbidden.
/// 5. Если роль == "Admin" — продолжаем выполнение метода контроллера.
/// 
/// ПРИМЕЧАНИЕ ДЛЯ ЗАЩИТЫ:
/// В продакшене роль берётся из JWT Claims (context.HttpContext.User.FindFirst("role")),
/// а не из заголовка. Заголовок используется здесь для простоты демонстрации.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AdminModAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Этот метод вызывается ПЕРЕД выполнением действия (Action) контроллера.
    /// context — содержит всё о текущем запросе: заголовки, тело, пользователь.
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Читаем роль из Claims (JWT-токен, если настроена авторизация)
        var claimRole = context.HttpContext.User.FindFirst("role")?.Value;

        // Если Claims нет (авторизация не настроена), читаем из заголовка
        // Это fallback для тестирования через Swagger/Postman без полного Auth.
        // В продакшене эту строку нужно удалить.
        if (string.IsNullOrEmpty(claimRole))
        {
            claimRole = context.HttpContext.Request.Headers["X-User-Role"].FirstOrDefault();
        }

        // Проверяем, является ли пользователь администратором
        if (!string.Equals(claimRole, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            // Немедленно прерываем выполнение и возвращаем 403 Forbidden.
            // Если context.Result присвоен — метод контроллера НЕ вызывается.
            context.Result = new ObjectResult(new
            {
                error = "Доступ запрещён. Требуются права Администратора.",
                requiredRole = "Admin",
                yourRole = string.IsNullOrEmpty(claimRole) ? "не определена" : claimRole
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            return; // Прерываем выполнение фильтра
        }

        // Если всё ок — передаём управление следующему в цепочке
        // base.OnActionExecuting() вызывает следующий фильтр или сам метод контроллера.
        base.OnActionExecuting(context);
    }
}
