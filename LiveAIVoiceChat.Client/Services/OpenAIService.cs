using System.Text.Json;
using System.Text;
using LiveAIVoiceChat.Client.Models;
using System;

namespace LiveAIVoiceChat.Client.Services
{
    /// <summary>
    /// Example usage of the <see cref="OpenAIService"/> class.
    /// </summary>
    /// <example>
    /// <code>
    /// // Client side call to OpenAIService and passing a recording
    /// // Stream response from OpenAI API using recognized speech
    /// private async Task StreamResponseFromOpenAI(string prompt)
    /// {
    ///     response = ""; // Clear previous response
    ///     await OpenAIService.GetStreamedResponseAsync(prompt, OnDataReceived);
    /// }
    /// </code>
    /// </example>
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        private StringBuilder _responseBuilder = new StringBuilder();
        private readonly ConfigurationService _configurationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIService"/> class.
        /// </summary>
        /// <param name="httpClient">An instance of <see cref="HttpClient"/> used to send HTTP requests.</param>
        public OpenAIService(HttpClient httpClient, ConfigurationService configurationService)
        {
            _httpClient = httpClient;
            _configurationService = configurationService;
            
        }

        /// <summary>
        /// Sends a prompt to the OpenAI API and processes the streamed response.
        /// </summary>
        /// <param name="prompt">The input prompt to send to the OpenAI API.</param>
        /// <param name="onDataReceived">A callback function to handle the data received from the API.</param>
        public async Task GetStreamedResponseAsync(string prompt, Action<string> onDataReceived)
        {
            var apiKey = await _configurationService.GetConfigurationAsync();
            _apiKey = apiKey.OpenAI.ApiKey;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] { new { role = "user", content = prompt } },
                stream = true
            };

            var requestContent = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );
            request.Content = requestContent;

            try
            {
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    onDataReceived($"Error: {response.StatusCode}");
                    return;
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (line.StartsWith("data:"))
                        {
                            var jsonData = line.Substring(5).Trim();

                            if (jsonData == "[DONE]")
                            {
                                // Send the accumulated content to the front end once the stream is complete
                                onDataReceived(_responseBuilder.ToString());
                                _responseBuilder.Clear(); // Clear the builder after sending
                                break; // End of stream
                            }

                            // Deserialize and handle the JSON response
                            HandleJsonResponse(jsonData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                onDataReceived($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Deserializes the JSON data and accumulates the content.
        /// </summary>
        /// <param name="jsonData">The JSON data string received from the API.</param>
        private void HandleJsonResponse(string jsonData)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Automatically convert to camelCase
                    AllowTrailingCommas = true
                };

                var responsePart = JsonSerializer.Deserialize<StreamResponse>(jsonData, options);

                if (responsePart?.Choices != null && responsePart.Choices.Count > 0)
                {
                    foreach (var choice in responsePart.Choices)
                    {
                        var content = choice.Delta?.Content;

                        if (!string.IsNullOrEmpty(content))
                        {
                            _responseBuilder.Append(content); // Accumulate the content
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse response: {ex.Message}");
            }
        }
    }
}
