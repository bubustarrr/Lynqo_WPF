using Lynqo_AdminWPF.Helpers; // <-- EZ OLDJA MEG A RELAYCOMMAND HIBÁT
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
                    MessageBox.Show("Sikeres belépés! Most történik az átirányítás...");

                    // Átváltunk a UI szálra az ablak létrehozásához
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            // 1. Létrehozzuk az új ablakot
                            var mainWindow = new Views.MainWindow();
                            mainWindow.DataContext = new MainViewModel(_api);

                            // 2. Beállítjuk ezt új Főablaknak (Így a WPF nem állítja le a programot!)
                            var oldWindow = Application.Current.MainWindow;
                            Application.Current.MainWindow = mainWindow;

                            // 3. Megjelenítjük az újat
                            mainWindow.Show();

                            // 4. Bezárjuk a régit
                            oldWindow?.Close();
                        }
                        catch (Exception winEx) // <--- EZ FOGJA MEGMONDANI MI A BAJ!
                        {
                            MessageBox.Show($"Kritikus hiba az ablak megnyitásakor:\n{winEx.Message}\n\n{winEx.StackTrace}");
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Sikertelen belépés! Hibás adatok.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hálózati/Szerver hiba:\n{ex.Message}");
            }
            finally { IsBusy = false; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}