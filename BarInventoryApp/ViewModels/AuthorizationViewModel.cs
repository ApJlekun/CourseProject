using BarInventoryApp.Constants;
using BarInventoryApp.Pages;
using BarInventoryApp.Services;
using BarInventoryApp.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BarInventoryApp.ViewModels;

/// <summary>
/// ViewModel для страницы авторизации.
/// </summary>
public class AuthorizationViewModel : INotifyPropertyChanged
{
    #region Поля

    private readonly AuthService _authService;
    private readonly MainViewModel _mainViewModel;
    private readonly IServiceProvider _serviceProvider;
    private string _login = string.Empty;
    private string _password = string.Empty;

    #endregion

    #region Свойства

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login
    {
        get => _login;
        set { _login = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Команда для выполнения входа в систему.
    /// </summary>
    public ICommand LoginCommand { get; }

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса AuthorizationViewModel.
    /// </summary>
    /// <param name="authService">Сервис аутентификации.</param>
    /// <param name="mainViewModel">Главная ViewModel для навигации.</param>
    /// <param name="serviceProvider">Провайдер сервисов.</param>
    public AuthorizationViewModel(AuthService authService, MainViewModel mainViewModel, IServiceProvider serviceProvider)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        LoginCommand = new RelayCommand(OnLogin);
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обработчик команды входа в систему.
    /// </summary>
    private async void OnLogin()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show(ApplicationConstants.Messages.LoginPasswordRequired);
            return;
        }

        try
        {
            var user = await _authService.AuthenticateAsync(Login, Password);
            if (user == null)
            {
                MessageBox.Show(ApplicationConstants.Messages.InvalidCredentials);
                return;
            }

            Session.CurrentUser = user;

            // Навигация в зависимости от роли пользователя
            switch (user.Role.Name)
            {
                case ApplicationConstants.Roles.Barmen:
                    _mainViewModel.NavigateTo<IngredientsPage>();
                    break;
                case ApplicationConstants.Roles.Manager:
                    _mainViewModel.NavigateTo<ManagerDashboardPage>();
                    break;
                case ApplicationConstants.Roles.Admin:
                    _mainViewModel.NavigateTo<AdminDashboardPage>();
                    break;
                default:
                    MessageBox.Show(ApplicationConstants.Messages.UnknownRole);
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(ApplicationConstants.Messages.LoginError, ex.Message),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    #endregion

    #region События

    /// <summary>
    /// Событие, возникающее при изменении значения свойства.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Вызывает событие PropertyChanged.
    /// </summary>
    /// <param name="propertyName">Имя измененного свойства.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
