namespace BarInventoryApp.Models;

/// <summary>
/// Представляет пользователя системы управления инвентарем бара.
/// </summary>
public partial class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Логин пользователя для входа в систему.
    /// </summary>
    public string Login { get; set; } = null!;

    /// <summary>
    /// Хеш пароля пользователя.
    /// Внимание: в текущей реализации хранится как открытый текст.
    /// Рекомендуется использовать хеширование (например, BCrypt).
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Идентификатор роли пользователя.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Коллекция заказов, созданных данным пользователем.
    /// </summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>
    /// Роль пользователя в системе (Barmen, Manager, Admin).
    /// </summary>
    public virtual Role Role { get; set; } = null!;
}
