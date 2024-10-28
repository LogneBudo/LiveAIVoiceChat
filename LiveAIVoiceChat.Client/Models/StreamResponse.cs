using LiveAIVoiceChat.Client.Models;

namespace LiveAIVoiceChat.Client.Models
{
    public class StreamResponse
    {
        public string? Id { get; set; }
        public string? Object { get; set; }
        public long Created { get; set; }
        public string? Model { get; set; }
        public List<Choice>? Choices { get; set; }
    }

}
