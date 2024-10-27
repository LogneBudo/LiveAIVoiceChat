using LiveAIVoiceChat.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace LiveAIVoiceChat.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices();
            builder.Services.AddScoped<ConfigurationService>();
            var host = builder.Build();
            
            // Register the OpenAIService with the API key
            builder.Services.AddScoped<OpenAIService>();

            await builder.Build().RunAsync();
        }
    }
}
