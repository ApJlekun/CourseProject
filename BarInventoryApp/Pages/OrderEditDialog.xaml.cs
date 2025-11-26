using BarInventoryApp.Models;
using BarInventoryApp.Services;
using BarInventoryApp.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BarInventoryApp.Pages
{
    public partial class OrderEditDialog : Window, INotifyPropertyChanged
    {
        private readonly Order? _order;
        private readonly OrderService _orderService;
        private readonly IngredientService _ingredientService;
        private List<Ingredient> _ingredients = new();
        private Ingredient? _selectedIngredient;
        private decimal _quantity = 1;
        private DateTime _orderDate = DateTime.Now;

        // Свойства для привязки
        public List<Ingredient> Ingredients
        {
            get => _ingredients;
            set { _ingredients = value; OnPropertyChanged(); }
        }

        public Ingredient? SelectedIngredient
        {
            get => _selectedIngredient;
            set { _selectedIngredient = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set
            {
                if (value > 0) _quantity = value;
                OnPropertyChanged();
            }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set { _orderDate = value; OnPropertyChanged(); }
        }

        public bool Confirmed { get; private set; }

        public OrderEditDialog(Order? order, OrderService orderService, IngredientService ingredientService)
        {
            InitializeComponent();
            _order = order;
            _orderService = orderService;
            _ingredientService = ingredientService;
            DataContext = this;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загружаем все ингредиенты для выбора
                Ingredients = await _ingredientService.GetAllAsync();

                if (_order != null)
                {
                    // Редактирование: подгружаем данные заказа
                    SelectedIngredient = Ingredients.FirstOrDefault(i => i.Id == _order.IngredientId);
                    Quantity = _order.Quantity;
                    OrderDate = _order.OrderDate;
                }
                else
                {
                    // Создание: выбираем первый ингредиент по умолчанию (если есть)
                    SelectedIngredient = Ingredients.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ингредиентов: {ex.Message}");
                Close();
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIngredient == null)
            {
                MessageBox.Show("Выберите ингредиент.");
                return;
            }

            if (Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля.");
                return;
            }

            try
            {
                if (_order == null)
                {
                    // Создаём новый заказ
                    var newOrder = new Order
                    {
                        IngredientId = SelectedIngredient.Id,
                        Quantity = Quantity,
                        OrderDate = OrderDate,
                        CreatedBy = Session.CurrentUser!.Id // гарантированно не null при входе
                    };
                    await _orderService.AddAsync(newOrder);
                }
                else
                {
                    // Обновляем существующий
                    _order.IngredientId = SelectedIngredient.Id;
                    _order.Quantity = Quantity;
                    _order.OrderDate = OrderDate;
                    await _orderService.UpdateAsync(_order);
                }

                Confirmed = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
