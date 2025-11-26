using BarInventoryApp.Models;
using BarInventoryApp.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BarInventoryApp.Pages
{
    public partial class IngredientEditDialog : Window, INotifyPropertyChanged
    {
        private readonly Ingredient? _ingredient;
        private readonly IngredientService _service;
        public bool Confirmed { get; private set; }

        private string _ingredientName = "";
        private decimal _quantity = 0;
        private string _unit = "";

        public string IngredientName
        {
            get => _ingredientName;
            set { _ingredientName = value; OnPropertyChanged(); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(); }
        }

        public IngredientEditDialog(Ingredient? ingredient, IngredientService service)
        {
            InitializeComponent();
            _ingredient = ingredient;
            _service = service;

            if (ingredient != null)
            {
                IngredientName = ingredient.Name;
                Quantity = ingredient.Quantity;
                Unit = ingredient.Unit;
            }

            DataContext = this;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IngredientName) || string.IsNullOrWhiteSpace(Unit) || Quantity < 0)
            {
                MessageBox.Show("Заполните все поля корректно.");
                return;
            }

            if (_ingredient == null)
            {
                await _service.AddAsync(new Ingredient { Name = IngredientName, Quantity = Quantity, Unit = Unit });
            }
            else
            {
                _ingredient.Name = IngredientName;
                _ingredient.Quantity = Quantity;
                _ingredient.Unit = Unit;
                await _service.UpdateAsync(_ingredient);
            }

            Confirmed = true;
            DialogResult = true;
            Close();
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
