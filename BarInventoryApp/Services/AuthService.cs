using BarInventoryApp.DataContexts;
using BarInventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarInventoryApp.Services;

/// <summary>
/// Сервис для аутентификации пользователей.
/// </summary>
public class AuthService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса AuthService.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public AuthService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Аутентифицирует пользователя по логину и паролю.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>Аутентифицированный пользователь или null, если аутентификация не удалась.</returns>
    /// <exception cref="ArgumentNullException">Если login или password равны null или пустые.</exception>
    public async Task<User?> AuthenticateAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Логин не может быть пустым.", nameof(login));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(password));

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Login == login);

        // Внимание: В текущей реализации пароль сравнивается в открытом виде.
        // В продакшене следует использовать хеширование паролей (например, BCrypt).
        if (user != null && user.PasswordHash == password)
            return user;

        return null;
    }
}
