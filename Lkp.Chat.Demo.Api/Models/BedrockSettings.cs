namespace Lkp.Chat.Demo.Api.Models
{
    public class BedrockSettings
    {
        public string KnowledgeBaseId { get; set; } = string.Empty;
        public int MaxResults { get; set; } = 3;
        public string SearchType { get; set; } = "Semantic";
    }
}
