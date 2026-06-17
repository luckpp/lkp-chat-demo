namespace Lkp.Chat.Demo.Api.Dto
{
    public class ChatDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<ChatItemDto> Items { get; set; } = new List<ChatItemDto>();
    }
}
