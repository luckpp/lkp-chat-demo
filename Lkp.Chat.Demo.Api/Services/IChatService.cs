using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Services
{
    public interface IChatService
    {
        Task<object> CreateAsync(CreateChatDto createChatDto);
        Task<object> GetAsync(string chatId);
        Task<object> UpdateAsync(string chatId, CreateChatItemDto createChatItemDto);
        Task DeleteAsync(string chatId);
    }
}
