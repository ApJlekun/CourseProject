using BarInventoryApp.DataContexts;
using BarInventoryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BarInventoryApp.Services;

/// <summary>
/// Сервис для работы с пользователями.
/// </summary>
public class UserService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса UserService.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public UserService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Получает всех пользователей с загруженными ролями.
    /// </summary>
    /// <returns>Список всех пользователей с ролями.</returns>
    public async Task<List<User>> GetAllWithRolesAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .ToListAsync();
    }

    /// <summary>
    /// Добавляет нового пользователя в базу данных.
    /// </summary>
    /// <param name="user">Пользователь для добавления.</param>
    /// <param name="plainPassword">Пароль в открытом виде.</param>
    /// <exception cref="ArgumentNullException">Если user или plainPassword равны null.</exception>
    /// <remarks>
    /// Внимание: В текущей реализации пароль сохраняется в открытом виде.
    /// В продакшене следует использовать хеширование паролей (например, BCrypt).
    /// </remarks>
    public async Task AddAsync(User user, string plainPassword)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(plainPassword));

        // Внимание: пароль сохраняется в открытом виде. В продакшене следует хешировать.
        user.PasswordHash = plainPassword;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Обновляет роль пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="newRoleId">Новый идентификатор роли.</param>
    /// <returns>True, если роль была обновлена; иначе false.</returns>
    public async Task<bool> UpdateRoleAsync(int userId, int newRoleId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.RoleId = newRoleId;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
