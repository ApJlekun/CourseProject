using BarInventoryApp.Constants;
using BarInventoryApp.Utils;
using BarInventoryApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BarInventoryApp.Pages;

/// <summary>
/// Страница управления ингредиентами.
/// </summary>
public partial class IngredientsPage : Page
{
    #region Поля

    private readonly MainViewModel _mainViewModel;

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса IngredientsPage.
    /// </summary>
    /// <param name="viewModel">ViewModel для страницы ингредиентов.</param>
    /// <param name="mainViewModel">Главная ViewModel для навигации.</param>
    public IngredientsPage(IngredientsViewModel viewModel, MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _mainViewModel = mainViewModel;
    }

    #endregion

    #region Обработчики событий

    /// <summary>
    /// Обработчик изменения текста в поле поиска.
    /// Фильтрация уже работает через Binding + PropertyChanged.
    /// </summary>
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Фильтрация уже работает через Binding + PropertyChanged
    }

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
