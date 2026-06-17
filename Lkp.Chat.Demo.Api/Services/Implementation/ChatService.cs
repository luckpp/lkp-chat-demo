using Lkp.Chat.Demo.Api.Dto;
using Lkp.Chat.Demo.Api.Models;
using Lkp.Chat.Demo.Api.Repositories;

namespace Lkp.Chat.Demo.Api.Services.Implementation
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repo;
        private readonly IRagService _ragService;
        private readonly IPromptService _promptService;
        private readonly IInferenceService _inferenceService;
        private readonly IRephraseInferenceService _rephraseInferenceService;

        public ChatService(
            IChatRepository repo,
            IRagService ragService,
            IPromptService promptService,
            IInferenceService inferenceService,
            IRephraseInferenceService rephraseInferenceService)
        {
            _repo = repo;
            _ragService = ragService;
            _promptService = promptService;
            _inferenceService = inferenceService;
            _rephraseInferenceService = rephraseInferenceService;
        }

        public async Task<ChatDto> CreateAsync(CreateChatDto createChatDto)
        {
            var documents = await _ragService.RetrieveDocumentsAsync(
                createChatDto.Content);

            var response = await _inferenceService.GenerateResponseAsync(
                    createChatDto.Content, 
                    [],
                    documents);

            var chatId = Guid.NewGuid().ToString();
            var chat = new ChatDto
            {
                Id = chatId,
                Name = createChatDto.Name,
                Items =
                [
                    new ChatItemDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChatId = chatId,
                        Content = createChatDto.Content,
                        Role = "user"
                    },
                    new ChatItemDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        ChatId = chatId,
                        Content = response,
                        Role = "assistant"
                    }
                ]
            };

            return await _repo.CreateAsync(chat);
        }

        public async Task<ChatDto?> GetAsync(string chatId)
        {
            var chat = await _repo.GetAsync(chatId);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with id '{chatId}' not found.");
            return chat;
        }

        public async Task<ChatItemDto?> UpdateAsync(string chatId, CreateChatItemDto createChatItemDto)
        {
            var chat = await _repo.GetAsync(chatId);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with id '{chatId}' not found.");

            var userIntent = await _rephraseInferenceService.DetermineUserIntentAsync(
                createChatItemDto.Content,
                chat.Items);

            var documents = await _ragService.RetrieveDocumentsAsync(userIntent);

            var response = await _inferenceService.GenerateResponseAsync(
                createChatItemDto.Content,
                chat.Items,
                documents);

            var userItem = new ChatItemDto
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                Content = createChatItemDto.Content,
                Role = "user"
            };

            var assistantItem = new ChatItemDto
            {
                Id = Guid.NewGuid().ToString(),
                ChatId = chatId,
                Content = response,
                Role = "assistant"
            };

            chat.Items.Add(userItem);
            chat.Items.Add(assistantItem);

            await _repo.UpdateAsync(chat);

            return assistantItem;
        }

        public async Task DeleteAsync(string chatId)
        {
            await _repo.DeleteAsync(chatId);
        }
    }
}
