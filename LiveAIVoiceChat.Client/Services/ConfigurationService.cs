using System.Net.Http.Json;
using LiveAIVoiceChat.Client.Models;

namespace LiveAIVoiceChat.Client.Services
{
    public class ConfigurationService
    {
        private readonly HttpClient _httpClient;

        public ConfigurationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AppSettings?> GetConfigurationAsync()
        {
            return await _httpClient.GetFromJsonAsync<AppSettings>("api/configuration");
        }

    }
}
