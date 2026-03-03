using Lynqo_AdminWPF.Services;
using Lynqo_AdminWPF.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Lynqo_AdminWPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object? _currentPage;
        public object? CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public ICommand ShowUsersCommand { get; }

        private readonly ApiClient _api;

        public MainViewModel(ApiClient api)
        {
            _api = api;
            ShowUsersCommand = new RelayCommand(_ => ShowUsers());
            ShowUsers();
        }

        private void ShowUsers()
        {
            CurrentPage = new UsersPage { DataContext = new UsersViewModel(_api) };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
