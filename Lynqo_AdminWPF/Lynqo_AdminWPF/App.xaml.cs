using Lynqo_AdminWPF.Services;
using Lynqo_AdminWPF.ViewModels;
using System.Windows;

namespace Lynqo_AdminWPF
{
    public partial class App : Application
    {
        private static readonly ApiClient _apiClient = new ApiClient("https://localhost:7118/");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var loginVM = new LoginViewModel(_apiClient);
            var loginWin = new Views.LoginWindow { DataContext = loginVM };
            loginWin.Show();
        }
    }
}