using BarInventoryApp.Utils;
using BarInventoryApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace BarInventoryApp.Pages
{
    public partial class UserPage : Page
    {
        private readonly MainViewModel _mainViewModel;

        public UserPage(UsersViewModel viewModel, MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _mainViewModel = mainViewModel;
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            _mainViewModel.NavigateTo<AdminDashboardPage>();
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            _mainViewModel.NavigateTo<AuthorizationPage>();
        }
    }
}
