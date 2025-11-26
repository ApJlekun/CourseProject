using BarInventoryApp.Constants;
using BarInventoryApp.Utils;
using BarInventoryApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BarInventoryApp.Pages;

/// <summary>
/// Страница управления заказами.
/// </summary>
public partial class OrdersPage : Page
{
    #region Поля

    private readonly MainViewModel _mainViewModel;

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса OrdersPage.
    /// </summary>
    /// <param name="viewModel">ViewModel для страницы заказов.</param>
    /// <param name="mainViewModel">Главная ViewModel для навигации.</param>
    public OrdersPage(OrdersViewModel viewModel, MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _mainViewModel = mainViewModel;
    }

    #endregion

    #region Обработчики событий

    /// <summary>
    /// Обработчик нажатия кнопки "Назад".
    /// </summary>
    private void OnBackClick(object sender, RoutedEventArgs e)
    {
        var role = Session.CurrentUser?.Role.Name;
        switch (role)
        {
            case ApplicationConstants.Roles.Admin:
                _mainViewModel.NavigateTo<AdminDashboardPage>();
                break;
            case ApplicationConstants.Roles.Manager:
                _mainViewModel.NavigateTo<ManagerDashboardPage>();
                break;
            default:
                _mainViewModel.NavigateTo<AuthorizationPage>();
                break;
        }
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Выход".
    /// </summary>
    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        Session.CurrentUser = null;
        _mainViewModel.NavigateTo<AuthorizationPage>();
    }

    #endregion
}
