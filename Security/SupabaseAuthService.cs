using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BE_01.Security
{
    public class SupabaseAuthService
    {
        private readonly HttpClient _httpClient;

        public SupabaseAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> TestConnection()
        {
            var response = await _httpClient.GetAsync("/auth/v1/settings");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<(bool Success, int StatusCode, string ResponseBody)> SignUp(string email, string password)
        {
            var payload = new { email, password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/v1/signup", content);
            var body = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, (int)response.StatusCode, body);
        }

        public async Task<(bool Success, int StatusCode, string ResponseBody)> Login(string email, string password)
        {
            var payload = new { email, password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/auth/v1/token?grant_type=password", content);
            var body = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, (int)response.StatusCode, body);
        }
    }
}