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
    }
}