using System.Net.Http.Json;

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

    public class AppSettings
    {
        public required OpenAISettings OpenAI { get; set; }
    }

    public class OpenAISettings
    {
        public required string ApiKey { get; set; }
    }
}
