using Lkp.Chat.Demo.Api.Models;

namespace Lkp.Chat.Demo.Api.Services;

public interface IPromptService
{
    public string BuildPrompt(
        string userInput,
        IEnumerable<RagDocument> ragDocuments,
        string chatHistory = "");
}
