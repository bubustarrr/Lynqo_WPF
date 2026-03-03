using Lynqo_AdminWPF.Models;
using Lynqo_AdminWPF.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Lynqo_AdminWPF.ViewModels
{
    public class UsersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ApiClient _api;

        public ObservableCollection<AdminUserDto> Users { get; } = new();

        private AdminUserDto? _selectedUser;
        public AdminUserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
                OnPropertyChanged(nameof(CanPromote));
                OnPropertyChanged(nameof(CanDemote));
                OnPropertyChanged(nameof(CanBan));
                OnPropertyChanged(nameof(CanUnban));
            }
        }

        public bool HasSelection => SelectedUser != null;
        public bool CanPromote => SelectedUser?.Role == "user";
        public bool CanDemote => SelectedUser?.Role == "admin";
        public bool CanBan => SelectedUser != null && !SelectedUser.IsBanned;
        public bool CanUnban => SelectedUser != null && SelectedUser.IsBanned;

        public string BanReason { get; set; } = "";
        public DateTime? BanUntil { get; set; }

        public ICommand PromoteCommand { get; }
        public ICommand DemoteCommand { get; }
        public ICommand ChangePictureCommand { get; }
        public ICommand BanCommand { get; }
        public ICommand UnbanCommand { get; }

        public UsersViewModel(ApiClient api)
        {
            _api = api;

            PromoteCommand = new RelayCommand(async _ => await ChangeRole("admin"), _ => CanPromote);
            DemoteCommand = new RelayCommand(async _ => await ChangeRole("user"), _ => CanDemote);
            ChangePictureCommand = new RelayCommand(async _ => await ChangePicture(), _ => HasSelection);
            BanCommand = new RelayCommand(async _ => await BanUser(), _ => CanBan);
            UnbanCommand = new RelayCommand(async _ => await UnbanUser(), _ => CanUnban);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            try
            {
                var list = await _api.GetUsersAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Clear();
                    foreach (var u in list) Users.Add(u);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba felhasználók betöltésekor: {ex.Message}");
            }
        }

        private async Task ChangeRole(string role)
        {
            if (SelectedUser == null) return;
            try
            {
                await _api.SetRoleAsync(SelectedUser.Id, role);
                SelectedUser.Role = role;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(CanPromote));
                OnPropertyChanged(nameof(CanDemote));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba szerep változtatásakor: {ex.Message}");
            }
        }

        private async Task ChangePicture()
        {
            if (SelectedUser == null) return;
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Képlapok|*.png;*.jpg;*.jpeg"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var url = await _api.UploadProfileImageAsync(dlg.FileName);
                    await _api.SetProfilePicAsync(SelectedUser.Id, url);
                    SelectedUser.ProfilePicUrl = url;
                    OnPropertyChanged(nameof(SelectedUser));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba kép feltöltésekor: {ex.Message}");
                }
            }
        }

        private async Task BanUser()
        {
            if (SelectedUser == null) return;
            try
            {
                await _api.BanUserAsync(SelectedUser.Id, BanReason, BanUntil);
                SelectedUser.IsBanned = true;
                SelectedUser.BanReason = BanReason;
                SelectedUser.BanUntil = BanUntil;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(CanBan));
                OnPropertyChanged(nameof(CanUnban));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba tiltáskor: {ex.Message}");
            }
        }

        private async Task UnbanUser()
        {
            if (SelectedUser == null) return;
            try
            {
                await _api.UnbanUserAsync(SelectedUser.Id);
                SelectedUser.IsBanned = false;
                SelectedUser.BanReason = null;
                SelectedUser.BanUntil = null;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(CanBan));
                OnPropertyChanged(nameof(CanUnban));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba feloldáskor: {ex.Message}");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
