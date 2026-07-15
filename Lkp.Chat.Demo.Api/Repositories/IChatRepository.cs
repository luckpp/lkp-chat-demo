using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Repositories;

public interface IChatRepository
{
    Task<ChatDto> CreateAsync(ChatDto chat);
    Task<IEnumerable<ChatSummaryDto>> GetAllAsync();
    Task<ChatDto?> GetAsync(string chatId);
    Task<ChatDto> UpdateAsync(ChatDto chat);
    Task DeleteAsync(string chatId);
}
