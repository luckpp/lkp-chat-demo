namespace Lkp.Chat.Demo.Api.Services;

public interface IInferenceService
{
    Task<string> GenerateResponseAsync(string prompt);
}
