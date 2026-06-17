using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Services;

public interface IRephraseInferenceService
{
    Task<string> DetermineUserIntentAsync(string userInput, IEnumerable<ChatItemDto> chatHistory);
}
