using Lkp.Chat.Demo.Api.Dto;
using Lkp.Chat.Demo.Api.Models;

namespace Lkp.Chat.Demo.Api.Services;

public interface IInferenceService
{
    Task<string> GenerateResponseAsync(
        string userInput,
        IEnumerable<ChatItemDto> history,
        IEnumerable<RagDocument> context);
}
