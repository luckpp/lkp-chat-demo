//using System.Collections.Concurrent;
//using Lkp.Chat.Demo.Api.Dto;

//namespace Lkp.Chat.Demo.Api.Repositories.Implementation;

//public class InMemoryChatRepository : IChatRepository
//{
//    private readonly ConcurrentDictionary<string, ChatDto> _store = new();

//    public Task<ChatDto> CreateAsync(CreateChatDto request)
//    {
//        var chatId = Guid.NewGuid().ToString();

//        var chat = new ChatDto
//        {
//            Id = chatId,
//            Name = request.Name,
//            Items =
//            [
//                new ChatItemDto
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    Content = request.Content
//                }
//            ]
//        };

//        _store[chatId] = chat;

//        return Task.FromResult(chat);
//    }

//    public Task<ChatDto?> GetAsync(string chatId)
//    {
//        _store.TryGetValue(chatId, out var item);
//        return Task.FromResult(item);
//    }

//    public Task<ChatItemDto> UpdateAsync(string chatId, CreateChatItemDto request)
//    {
//        var chatItem = new ChatItemDto
//        {
//            Id = Guid.NewGuid().ToString(),
//            Content = request.Content
//        };

//        if (_store.TryGetValue(chatId, out var item))
//        {
//            item.Items.Add(chatItem);
//        }

//        return Task.FromResult(chatItem);
//    }

//    public Task DeleteAsync(string chatId)
//    {
//        _store.TryRemove(chatId, out _);
//        return Task.CompletedTask;
//    }
//}
