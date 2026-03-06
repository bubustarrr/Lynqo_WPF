using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Lynqo_AdminWPF.Services;
using Lynqo_AdminWPF.Views;

namespace Lynqo_AdminWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Page? _currentPage;
        private readonly ApiClient _api;

        public Page? CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public MainViewModel(ApiClient api)
        {
            _api = api;
            ShowUsersPage(); // Alapból a felhasználókat mutatjuk
        }

        public void ShowUsersPage()
        {
            var vm = new UsersViewModel(_api);
            CurrentPage = new UsersPage { DataContext = vm };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}