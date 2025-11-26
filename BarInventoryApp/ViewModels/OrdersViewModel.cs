using BarInventoryApp.Constants;
using BarInventoryApp.Models;
using BarInventoryApp.Pages;
using BarInventoryApp.Services;
using BarInventoryApp.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace BarInventoryApp.ViewModels;

/// <summary>
/// ViewModel для страницы управления заказами.
/// </summary>
public class OrdersViewModel : INotifyPropertyChanged
{
    #region Поля

    private readonly OrderService _orderService;
    private readonly IngredientService _ingredientService;
    private readonly UserService _userService;
    private readonly ExcelExportService _excelService;
    private readonly MainViewModel _mainViewModel;
    private string _filter = string.Empty;
    private List<Order> _allOrders = new();

    #endregion

    #region Свойства

    /// <summary>
    /// Коллекция отфильтрованных заказов для отображения.
    /// </summary>
    public ObservableCollection<Order> Orders { get; } = new();

    /// <summary>
    /// Текст фильтра для поиска заказов.
    /// </summary>
    public string Filter
    {
        get => _filter;
        set { _filter = value; OnPropertyChanged(); ApplyFilter(); }
    }

    /// <summary>
    /// Выбранный заказ в списке (для привязки выделения в DataGrid).
    /// </summary>
    public Order? SelectedItem { get; set; }

    #endregion

    #region Команды

    /// <summary>
    /// Команда для добавления нового заказа.
    /// </summary>
    public ICommand AddCommand { get; }

    /// <summary>
    /// Команда для редактирования выбранного заказа.
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Команда для удаления выбранного заказа.
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Команда для экспорта заказов в Excel.
    /// </summary>
    public ICommand ExportCommand { get; }

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса OrdersViewModel.
    /// </summary>
    /// <param name="orderService">Сервис для работы с заказами.</param>
    /// <param name="ingredientService">Сервис для работы с ингредиентами.</param>
    /// <param name="userService">Сервис для работы с пользователями.</param>
    /// <param name="excelService">Сервис для экспорта в Excel.</param>
    /// <param name="mainViewModel">Главная ViewModel для навигации.</param>
    public OrdersViewModel(
        OrderService orderService,
        IngredientService ingredientService,
        UserService userService,
        ExcelExportService excelService,
        MainViewModel mainViewModel)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _ingredientService = ingredientService ?? throw new ArgumentNullException(nameof(ingredientService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));

        AddCommand = new RelayCommand(OnAdd);
        EditCommand = new RelayCommand(OnEdit);
        DeleteCommand = new RelayCommand(OnDelete);
        ExportCommand = new RelayCommand(OnExport);

        LoadOrders();
    }

    #endregion

    #region Методы

    /// <summary>
    /// Загружает все заказы из базы данных.
    /// </summary>
    private async void LoadOrders()
    {
        try
        {
            _allOrders = await _orderService.GetAllWithDetailsAsync();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(ApplicationConstants.Messages.LoadingOrdersError, ex.Message),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Применяет фильтр к списку заказов.
    /// </summary>
    private void ApplyFilter()
    {
        var filtered = _allOrders
            .Where(o => string.IsNullOrEmpty(Filter) ||
                        (o.Ingredient?.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (o.CreatedByNavigation?.Login.Contains(Filter, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        Orders.Clear();
        foreach (var order in filtered)
        {
            Orders.Add(order);
        }
    }

    /// <summary>
    /// Обработчик команды добавления нового заказа.
    /// </summary>
    private void OnAdd()
    {
        var dialog = new OrderEditDialog(null, _orderService, _ingredientService);
        if (dialog.ShowDialog() == true)
        {
            LoadOrders();
        }
    }

    /// <summary>
    /// Обработчик команды редактирования заказа.
    /// </summary>
    private void OnEdit()
    {
        if (SelectedItem is Order selected)
        {
            var dialog = new OrderEditDialog(selected, _orderService, _ingredientService);
            if (dialog.ShowDialog() == true)
            {
                LoadOrders();
            }
        }
        else
        {
            MessageBox.Show(ApplicationConstants.Messages.SelectOrderForEdit);
        }
    }

    /// <summary>
    /// Обработчик команды удаления заказа.
    /// </summary>
    private async void OnDelete()
    {
        if (SelectedItem is Order selected)
        {
            var result = MessageBox.Show(
                string.Format(ApplicationConstants.Messages.DeleteOrderConfirmation, selected.OrderDate.ToString("G")),
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _orderService.DeleteAsync(selected.Id);
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка удаления заказа: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }

    /// <summary>
    /// Обработчик команды экспорта заказов в Excel.
    /// </summary>
    private void OnExport()
    {
        try
        {
            _excelService.ExportOrders(_allOrders);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка экспорта в Excel: {ex.Message}",
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
