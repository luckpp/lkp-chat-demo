using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Services
{
    public interface IChatService
    {
        Task<ChatDto> CreateAsync(CreateChatDto createChatDto);
        Task<ChatDto?> GetAsync(string chatId);
        Task<ChatItemDto?> UpdateAsync(string chatId, CreateChatItemDto createChatItemDto);
        Task DeleteAsync(string chatId);
    }
}
