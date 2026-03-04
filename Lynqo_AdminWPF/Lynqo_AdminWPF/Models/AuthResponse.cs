namespace Lynqo_AdminWPF.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public AuthUser User { get; set; } = null!;
    }

    public class AuthUser
    {
        public string Username { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
    }
}