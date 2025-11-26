using BarInventoryApp.Constants;
using BarInventoryApp.Models;
using BarInventoryApp.Services;
using BarInventoryApp.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BarInventoryApp.ViewModels;

/// <summary>
/// ViewModel для страницы управления пользователями.
/// </summary>
public class UsersViewModel : INotifyPropertyChanged
{
    #region Поля

    private readonly UserService _userService;
    private readonly List<Role> _roles;
    private User? _selectedUser;
    private string _login = string.Empty;
    private string _password = string.Empty;
    private int _selectedRoleId;

    #endregion

    #region Свойства

    /// <summary>
    /// Коллекция пользователей для отображения.
    /// </summary>
    public ObservableCollection<User> Users { get; } = new();

    /// <summary>
    /// Коллекция ролей для выбора.
    /// </summary>
    public ObservableCollection<Role> Roles { get; }

    /// <summary>
    /// Выбранный пользователь для редактирования.
    /// </summary>
    public User? SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            if (value != null)
            {
                Login = value.Login;
                SelectedRoleId = value.RoleId;
            }
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Логин нового пользователя или редактируемого пользователя.
    /// </summary>
    public string Login
    {
        get => _login;
        set { _login = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Пароль нового пользователя.
    /// </summary>
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Идентификатор выбранной роли.
    /// </summary>
    public int SelectedRoleId
    {
        get => _selectedRoleId;
        set { _selectedRoleId = value; OnPropertyChanged(); }
    }

    #endregion

    #region Команды

    /// <summary>
    /// Команда для сохранения изменений роли выбранного пользователя.
    /// </summary>
    public ICommand SaveUserCommand { get; }

    /// <summary>
    /// Команда для создания нового пользователя.
    /// </summary>
    public ICommand CreateUserCommand { get; }

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса UsersViewModel.
    /// </summary>
    /// <param name="userService">Сервис для работы с пользователями.</param>
    public UsersViewModel(UserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        // Инициализация списка ролей (предполагаем, что роли неизменны и малочисленны)
        _roles = new List<Role>
        {
            new() { Id = ApplicationConstants.RoleIds.Barmen, Name = ApplicationConstants.Roles.Barmen },
            new() { Id = ApplicationConstants.RoleIds.Manager, Name = ApplicationConstants.Roles.Manager },
            new() { Id = ApplicationConstants.RoleIds.Admin, Name = ApplicationConstants.Roles.Admin }
        };
        Roles = new ObservableCollection<Role>(_roles);

        SaveUserCommand = new RelayCommand(OnSaveUser);
        CreateUserCommand = new RelayCommand(OnCreateUser);

        LoadUsers();
    }

    #endregion

    #region Методы

    /// <summary>
    /// Загружает всех пользователей из базы данных.
    /// </summary>
    private async void LoadUsers()
    {
        try
        {
            var users = await _userService.GetAllWithRolesAsync();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(ApplicationConstants.Messages.LoadingUsersError, ex.Message),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Обработчик команды сохранения изменений роли пользователя.
    /// </summary>
    private async void OnSaveUser()
    {
        if (SelectedUser == null)
            return;

        try
        {
            var updated = await _userService.UpdateRoleAsync(SelectedUser.Id, SelectedRoleId);
            if (updated)
            {
                MessageBox.Show(
                    ApplicationConstants.Messages.RoleUpdated,
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                LoadUsers();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка обновления роли: {ex.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Обработчик команды создания нового пользователя.
    /// </summary>
    private async void OnCreateUser()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show(ApplicationConstants.Messages.LoginPasswordRequiredForUser);
            return;
        }

        if (Users.Any(u => u.Login == Login))
        {
            MessageBox.Show(ApplicationConstants.Messages.UserExists);
            return;
        }

        try
        {
            var newUser = new User
            {
                Login = Login,
                RoleId = SelectedRoleId
            };

            await _userService.AddAsync(newUser, Password);
            MessageBox.Show(
                ApplicationConstants.Messages.UserCreated,
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            LoadUsers();

            // Очистка полей ввода
            Login = string.Empty;
            Password = string.Empty;
            OnPropertyChanged(nameof(Login));
            OnPropertyChanged(nameof(Password));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка создания пользователя: {ex.Message}",
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
