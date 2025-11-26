using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BarInventoryApp.Utils;

/// <summary>
/// Конвертер для преобразования null значения в Visibility.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Преобразует значение в Visibility.
    /// Если значение null, возвращает Collapsed, иначе Visible.
    /// </summary>
    /// <param name="value">Значение для преобразования.</param>
    /// <param name="targetType">Тип целевого свойства.</param>
    /// <param name="parameter">Параметр конвертера (не используется).</param>
    /// <param name="culture">Информация о культуре.</param>
    /// <returns>Visibility.Collapsed, если value равен null; иначе Visibility.Visible.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Обратное преобразование не поддерживается.
    /// </summary>
    /// <param name="value">Значение для обратного преобразования.</param>
    /// <param name="targetType">Тип целевого свойства.</param>
    /// <param name="parameter">Параметр конвертера (не используется).</param>
    /// <param name="culture">Информация о культуре.</param>
    /// <returns>Всегда выбрасывает NotImplementedException.</returns>
    /// <exception cref="NotImplementedException">Всегда.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается.");
    }
}
