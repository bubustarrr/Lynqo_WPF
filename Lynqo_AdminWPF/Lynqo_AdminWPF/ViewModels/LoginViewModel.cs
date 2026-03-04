using Lynqo_AdminWPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lynqo_AdminWPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _api;
        private string _username = "";
        private string _password = "";
        private bool _isBusy;

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiClient api)
        {
            _api = api;
            LoginCommand = new RelayCommand(async _ => await Login(), _ => !IsBusy);
        }

        private async Task Login()
        {
            IsBusy = true;
            try
            {
                bool success = await _api.LoginAsync(Username, Password);
                if (success)
                {
                    var usersVM = new UsersViewModel(_api);
                    var mainWindow = new Views.MainWindow();
                    mainWindow.DataContext = usersVM;
                    mainWindow.Show();

                    Application.Current.MainWindow.Close();
                    Application.Current.MainWindow = mainWindow;
                }
                else
                {
                    MessageBox.Show("Sikertelen belépés!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}");
            }
            finally { IsBusy = false; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}