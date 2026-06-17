using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Models
{
    public class FullPrompt
    {
        public string SystemPrompt { get; set; } = "";
        public List<ChatItemDto> History { get; set; } = new();
        public List<RagDocument> Context { get; set; } = new();
        public string UserInput { get; set; } = "";
    }
}
