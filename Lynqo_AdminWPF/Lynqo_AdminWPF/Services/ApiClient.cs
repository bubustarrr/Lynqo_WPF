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
        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }

        public ApiClient(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        private void ApplyAuthHeader()
        {
            if (!string.IsNullOrEmpty(AccessToken))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", AccessToken);
        }

        public async Task<bool> LoginAsync(string userOrEmail, string password)
        {
            var body = new { usernameOrEmail = userOrEmail, password };
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            var resp = await _http.PostAsync("api/auth/login",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (!resp.IsSuccessStatusCode)
                return false;

            var content = await resp.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions)!;
            AccessToken = auth.Token;
            RefreshToken = auth.RefreshToken;
            ApplyAuthHeader();

            return JwtHasAdminRole(auth.Token);
        }

        private bool JwtHasAdminRole(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) return false;

            string Pad(string s)
            {
                int padding = 4 - (s.Length % 4);
                if (padding == 4) return s;
                return s + new string('=', padding);
            }

            var payloadJson = Encoding.UTF8.GetString(
                Convert.FromBase64String(Pad(parts[1])));

            using var doc = JsonDocument.Parse(payloadJson);
            return doc.RootElement.TryGetProperty("role", out var roleProp) &&
                   roleProp.GetString() == "admin";
        }

        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            ApplyAuthHeader();
            var resp = await _http.GetAsync("api/admin/users");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AdminUserDto>>(json, _jsonOptions)!;
        }

        public async Task SetRoleAsync(int userId, string role)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(new { role }, _jsonOptions);
            var resp = await _http.PatchAsync($"api/admin/users/{userId}/role",
                new StringContent(json, Encoding.UTF8, "application/json"));
            resp.EnsureSuccessStatusCode();
        }

        public async Task BanUserAsync(int userId, string? reason, DateTime? until)
        {
            ApplyAuthHeader();

            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(reason))
                query.Add($"reason={Uri.EscapeDataString(reason)}");
            if (until.HasValue)
                query.Add($"bannedUntil={Uri.EscapeDataString(until.Value.ToString("o"))}");

            var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var resp = await _http.PostAsync($"api/admin/ban/{userId}{qs}", null);
            resp.EnsureSuccessStatusCode();
        }

        public async Task UnbanUserAsync(int userId)
        {
            ApplyAuthHeader();
            var resp = await _http.DeleteAsync($"api/admin/ban/{userId}");
            resp.EnsureSuccessStatusCode();
        }

        public async Task<string> UploadProfileImageAsync(string filePath)
        {
            ApplyAuthHeader();
            using var form = new MultipartFormDataContent();
            var bytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            form.Add(fileContent, "file", Path.GetFileName(filePath));
            form.Add(new StringContent("image"), "fileType");

            var resp = await _http.PostAsync("api/media/upload", form);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("fileUrl").GetString()!;
        }

        public async Task SetProfilePicAsync(int userId, string fileUrl)
        {
            ApplyAuthHeader();
            var json = JsonSerializer.Serialize(new { profilePicUrl = fileUrl }, _jsonOptions);
            var resp = await _http.PatchAsync($"api/admin/users/{userId}/profile-picture",
                new StringContent(json, Encoding.UTF8, "application/json"));
            resp.EnsureSuccessStatusCode();
        }
    }
}
