using System;

namespace Lynqo_AdminWPF.Models
{
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? ProfilePicUrl { get; set; }
        public bool IsPremium { get; set; }
        public int Hearts { get; set; }
        public int Coins { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public string? BanReason { get; set; }
        public DateTime? BanUntil { get; set; }
    }
}
