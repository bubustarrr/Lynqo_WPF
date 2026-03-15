using Lynqo_AdminWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lynqo_AdminWPF.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public string? AccessToken { get; private set; }

        public ApiClient(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        private void ApplyAuthHeader()
        {
            if (!string.IsNullOrEmpty(AccessToken))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public virtual async Task<bool> LoginAsync(string userOrEmail, string password)
        {
            try
            {
                var body = new { usernameOrEmail = userOrEmail, password };
                var json = JsonSerializer.Serialize(body, _jsonOptions);
                var resp = await _http.PostAsync("api/auth/login", new StringContent(json, Encoding.UTF8, "application/json"));
                if (!resp.IsSuccessStatusCode) return false;
                var content = await resp.Content.ReadAsStringAsync();
                var auth = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);
                if (auth == null) return false;
                AccessToken = auth.Token;
                ApplyAuthHeader();
                return true;
            }
            catch { return false; }
        }

        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            ApplyAuthHeader();
            var resp = await _http.GetAsync("api/admin/users");
            resp.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<List<AdminUserDto>>(await resp.Content.ReadAsStringAsync(), _jsonOptions) ?? new List<AdminUserDto>();
        }

        public async Task SetRoleAsync(int userId, string role)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(new { role }, _jsonOptions);
            await _http.PatchAsync($"api/admin/users/{userId}/role", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        // <-- ITT KAPTA MEG AZ ÚJ PARAMÉTERT
        public async Task GrantSubscriptionAsync(int userId, int months, bool autoRenew)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(new { durationMonths = months, autoRenew = autoRenew }, _jsonOptions);
            await _http.PostAsync($"api/admin/users/{userId}/subscription", new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public async Task RevokeSubscriptionAsync(int userId)
        {
            ApplyAuthHeader();
            await _http.DeleteAsync($"api/admin/users/{userId}/subscription");
        }

        public async Task BanUserAsync(int userId, string? reason, DateTime? until)
        {
            ApplyAuthHeader();
            var query = $"?reason={Uri.EscapeDataString(reason ?? "")}";
            if (until.HasValue) query += $"&bannedUntil={until.Value:o}";
            await _http.PostAsync($"api/admin/ban/{userId}{query}", null);
        }

        public async Task UnbanUserAsync(int userId)
        {
            ApplyAuthHeader();
            await _http.DeleteAsync($"api/admin/ban/{userId}");
        }

        public async Task<string> UploadProfileImageAsync(string filePath)
        {
            ApplyAuthHeader();
            using var form = new MultipartFormDataContent();
            var bytes = await File.ReadAllBytesAsync(filePath);
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            form.Add(content, "file", Path.GetFileName(filePath));
            var resp = await _http.PostAsync("api/media/upload", form);
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("fileUrl").GetString()!;
        }

        public async Task SetProfilePicAsync(int userId, string fileUrl)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(new { profilePicUrl = fileUrl }, _jsonOptions);
            await _http.PatchAsync($"api/admin/users/{userId}/profile-picture", new StringContent(json, Encoding.UTF8, "application/json"));
        }
    }
}