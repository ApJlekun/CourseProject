using BarInventoryApp.Models;

namespace BarInventoryApp.Utils;

/// <summary>
/// Представляет сеанс текущего пользователя в приложении.
/// </summary>
public static class Session
{
    /// <summary>
    /// Текущий авторизованный пользователь.
    /// </summary>
    public static User? CurrentUser { get; set; }
}
