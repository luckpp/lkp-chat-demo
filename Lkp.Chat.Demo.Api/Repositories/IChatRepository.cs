using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Repositories;

public interface IChatRepository
{
    Task<ChatDto> CreateAsync(ChatDto chat);
    Task<ChatDto?> GetAsync(string chatId);
    Task<ChatItemDto> UpdateAsync(string chatId, CreateChatItemDto request);
    Task DeleteAsync(string chatId);
}
