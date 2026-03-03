using Lynqo_AdminWPF.ViewModels;
using System.Windows;

namespace Lynqo_AdminWPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public LoginWindow()
        {
            InitializeComponent();
            _vm = new LoginViewModel();
            DataContext = _vm;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            _vm.Password = PwdBox.Password;
            if (await _vm.LoginAsync())
            {
                var main = new MainWindow(_vm.ApiClient);
                main.Show();
                Close();
            }
        }
    }
}
