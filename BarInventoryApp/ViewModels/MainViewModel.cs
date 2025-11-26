using BarInventoryApp.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BarInventoryApp.ViewModels;

/// <summary>
/// Главная ViewModel, управляющая навигацией между страницами приложения.
/// </summary>
public class MainViewModel
{
    private Frame? _frame;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Инициализирует новый экземпляр класса MainViewModel.
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов для создания страниц.</param>
    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Устанавливает Frame для навигации и переходит на страницу авторизации.
    /// </summary>
    /// <param name="frame">Frame для отображения страниц.</param>
    public void SetFrame(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        NavigateTo<AuthorizationPage>();
    }

    /// <summary>
    /// Переходит на указанную страницу типа T.
    /// </summary>
    /// <typeparam name="T">Тип страницы для навигации.</typeparam>
    /// <exception cref="InvalidOperationException">Если Frame не установлен.</exception>
    public void NavigateTo<T>() where T : Page
    {
        if (_frame == null)
            throw new InvalidOperationException("Frame не установлен. Вызовите SetFrame перед навигацией.");

        var page = _serviceProvider.GetRequiredService<T>();
        _frame.Content = page;
    }

    /// <summary>
    /// Переходит на указанную страницу.
    /// </summary>
    /// <param name="page">Страница для навигации.</param>
    /// <exception cref="InvalidOperationException">Если Frame не установлен.</exception>
    /// <exception cref="ArgumentNullException">Если page равен null.</exception>
    public void Navigate(Page page)
    {
        if (_frame == null)
            throw new InvalidOperationException("Frame не установлен. Вызовите SetFrame перед навигацией.");

        if (page == null)
            throw new ArgumentNullException(nameof(page));

        _frame.Content = page;
    }
}
