using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lynqo_AdminWPF.Models
{
    // Hozzáadtuk az INotifyPropertyChanged interfészt
    public class AdminUserDto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Ez a metódus szól a WPF-nek, hogy frissítse a képernyőt
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // --- Sima adatok (amiket nem módosítunk futás közben a listában) ---
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public bool IsPremium { get; set; }
        public int Hearts { get; set; }
        public int Coins { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? BanReason { get; set; }
        public DateTime? BanUntil { get; set; }

        // --- "Okos" adatok (amik változhatnak, és frissíteniük kell a UI-t) ---

        private bool _isBanned;
        public bool IsBanned
        {
            get => _isBanned;
            set
            {
                if (_isBanned != value)
                {
                    _isBanned = value;
                    OnPropertyChanged(); // Frissül a pipa a DataGridben!
                }
            }
        }

        private string _role = null!;
        public string Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged(); // Frissül a szöveg a DataGridben!
                }
            }
        }

        private string? _profilePicUrl;
        public string? ProfilePicUrl
        {
            get => _profilePicUrl;
            set
            {
                if (_profilePicUrl != value)
                {
                    _profilePicUrl = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}