using Lkp.Chat.Demo.Api.Models;

namespace Lkp.Chat.Demo.Api.Services.Implementation
{
    public class PromptService : IPromptService
    {
        public string BuildPrompt(string userInput, IEnumerable<RagDocument> ragDocuments, string chatHistory = "")
        {
            throw new NotImplementedException();
        }
    }
}
