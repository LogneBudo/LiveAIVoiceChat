namespace LiveAIVoiceChat.Client.Models
{
    public class Choice
    {
        public int Index { get; set; }
        public Delta? Delta { get; set; }
        public object? Logprobs { get; set; }  // Nullable property
        public string? FinishReason { get; set; }  // Nullable property
    }
}
