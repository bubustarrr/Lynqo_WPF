using Lynqo_AdminWPF.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Lynqo_AdminWPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string UsernameOrEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public ApiClient ApiClient { get; } =
            new ApiClient("https://localhost:5001/"); // ITT ÁLLÍtsd be az API URL-t!

        public async Task<bool> LoginAsync()
        {
            ErrorMessage = "";
            OnPropertyChanged(nameof(ErrorMessage));

            var ok = await ApiClient.LoginAsync(UsernameOrEmail, Password);
            if (!ok)
            {
                ErrorMessage = "Hibás belépési adatok vagy nincs admin jog.";
                OnPropertyChanged(nameof(ErrorMessage));
            }
            return ok;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
