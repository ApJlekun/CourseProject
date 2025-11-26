namespace BarInventoryApp.Models;

/// <summary>
/// Представляет роль пользователя в системе (Barmen, Manager, Admin).
/// </summary>
public partial class Role
{
    /// <summary>
    /// Уникальный идентификатор роли.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название роли.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Коллекция пользователей, имеющих данную роль.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
