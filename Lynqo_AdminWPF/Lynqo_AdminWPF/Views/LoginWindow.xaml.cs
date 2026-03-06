using System.Windows;
using System.Windows.Controls;
using Lynqo_AdminWPF.ViewModels; // Fontos, hogy ez itt legyen!

namespace Lynqo_AdminWPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Ez a metódus figyeli, ha gépelsz a jelszó mezőbe
        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Ha a DataContext be van állítva a LoginViewModel-re, frissítjük a jelszavát
            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}