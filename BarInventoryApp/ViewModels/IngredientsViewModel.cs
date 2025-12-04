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
/// ViewModel для страницы управления ингредиентами.
/// </summary>
public class IngredientsViewModel : INotifyPropertyChanged
{
    #region Поля

    private readonly IngredientService _service;
    private readonly MainViewModel _mainViewModel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ExcelImportService _importService;
    private string _filter = string.Empty;
    private List<Ingredient> _allIngredients = new();

    #endregion

    #region Свойства

    /// <summary>
    /// Коллекция отфильтрованных ингредиентов для отображения.
    /// </summary>
    public ObservableCollection<Ingredient> Ingredients { get; } = new();

    /// <summary>
    /// Текст фильтра для поиска ингредиентов.
    /// </summary>
    public string Filter
    {
        get => _filter;
        set { _filter = value; OnPropertyChanged(); ApplyFilter(); }
    }

    /// <summary>
    /// Выбранный ингредиент в списке (для привязки выделения в DataGrid).
    /// </summary>
    public Ingredient? SelectedItem { get; set; }

    #endregion

    #region Команды

    /// <summary>
    /// Команда для добавления нового ингредиента.
    /// </summary>
    public ICommand? AddCommand { get; }

    /// <summary>
    /// Команда для редактирования выбранного ингредиента.
    /// </summary>
    public ICommand? EditCommand { get; }

    /// <summary>
    /// Команда для удаления выбранного ингредиента.
    /// </summary>
    public ICommand? DeleteCommand { get; }

    /// <summary>
    /// Команда для импорта ингредиентов из Excel.
    /// </summary>
    public ICommand? ImportCommand { get; }

    #endregion

    #region Конструктор

    /// <summary>
    /// Инициализирует новый экземпляр класса IngredientsViewModel.
    /// </summary>
    /// <param name="service">Сервис для работы с ингредиентами.</param>
    /// <param name="mainViewModel">Главная ViewModel для навигации.</param>
    /// <param name="serviceProvider">Провайдер сервисов.</param>
    /// <param name="importService">Сервис для импорта из Excel.</param>
    public IngredientsViewModel(IngredientService service, MainViewModel mainViewModel, IServiceProvider serviceProvider, ExcelImportService importService)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _importService = importService ?? throw new ArgumentNullException(nameof(importService));

        // Команды доступны только администраторам
        var role = Session.CurrentUser?.Role.Name;
        if (role == ApplicationConstants.Roles.Admin)
        {
            AddCommand = new RelayCommand(OnAdd);
            EditCommand = new RelayCommand(OnEdit);
            DeleteCommand = new RelayCommand(OnDelete);
            ImportCommand = new RelayCommand(OnImport);
        }

        LoadIngredients();
    }

    #endregion

    #region Методы

    /// <summary>
    /// Загружает все ингредиенты из базы данных.
    /// </summary>
    private async void LoadIngredients()
    {
        try
        {
            _allIngredients = await _service.GetAllAsync();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                string.Format(ApplicationConstants.Messages.LoadingIngredientsError, ex.Message),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Применяет фильтр к списку ингредиентов.
    /// </summary>
    private void ApplyFilter()
    {
        var filtered = _allIngredients
            .Where(i => string.IsNullOrEmpty(Filter) ||
                        i.Name.Contains(Filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Ingredients.Clear();
        foreach (var ingredient in filtered)
        {
            Ingredients.Add(ingredient);
        }
    }

    /// <summary>
    /// Обработчик команды добавления нового ингредиента.
    /// </summary>
    private void OnAdd()
    {
        var dialog = new IngredientEditDialog(null, _service);
        if (dialog.ShowDialog() == true)
        {
            LoadIngredients();
        }
    }

    /// <summary>
    /// Обработчик команды редактирования ингредиента.
    /// </summary>
    private void OnEdit()
    {
        if (SelectedItem is Ingredient selected)
        {
            var dialog = new IngredientEditDialog(selected, _service);
            if (dialog.ShowDialog() == true)
            {
                LoadIngredients();
            }
        }
        else
        {
            MessageBox.Show(ApplicationConstants.Messages.SelectIngredientForEdit);
        }
    }

    /// <summary>
    /// Обработчик команды удаления ингредиента.
    /// </summary>
    private async void OnDelete()
    {
        if (SelectedItem is Ingredient selected)
        {
            var result = MessageBox.Show(
                string.Format(ApplicationConstants.Messages.DeleteIngredientConfirmation, selected.Name),
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _service.DeleteAsync(selected.Id);
                    LoadIngredients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка удаления ингредиента: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }

    /// <summary>
    /// Обработчик команды импорта ингредиентов из Excel.
    /// </summary>
    private async void OnImport()
    {
        try
        {
            // Показываем инструкции по импорту
            _importService.ShowImportInstructions();

            // Импортируем данные из файла
            var importedIngredients = _importService.ImportIngredients();
            if (importedIngredients == null || importedIngredients.Count == 0)
                return;

            int addedCount = 0;
            int updatedCount = 0;

            // Обрабатываем каждый импортированный ингредиент
            foreach (var importedIngredient in importedIngredients)
            {
                // Ищем существующий ингредиент с таким же названием
                var existingIngredient = _allIngredients.FirstOrDefault(i =>
                    i.Name.Equals(importedIngredient.Name, StringComparison.OrdinalIgnoreCase));

                if (existingIngredient != null)
                {
                    // Обновляем существующий
                    existingIngredient.Quantity = importedIngredient.Quantity;
                    existingIngredient.Unit = importedIngredient.Unit;
                    await _service.UpdateAsync(existingIngredient);
                    updatedCount++;
                }
                else
                {
                    // Добавляем новый
                    await _service.AddAsync(importedIngredient);
                    addedCount++;
                }
            }

            // Перезагружаем список
            LoadIngredients();

            // Показываем результат
            MessageBox.Show(
                $"Импорт завершен успешно!\nДобавлено: {addedCount}\nОбновлено: {updatedCount}",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка импорта: {ex.Message}",
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
