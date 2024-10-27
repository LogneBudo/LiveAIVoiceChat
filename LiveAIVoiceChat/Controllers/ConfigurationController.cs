using LiveAIVoiceChat.Client.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiveAIVoiceChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetConfiguration()
        {
            var apiKey = _configuration["OPENAI_API_KEY"];
            var appSettings = new AppSettings
            {
                OpenAI = new OpenAISettings
                {
                    ApiKey = apiKey
                }
            };
            return Ok(appSettings);
        }
    }
}
