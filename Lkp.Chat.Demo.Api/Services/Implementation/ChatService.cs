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

        public ChatService(
            IChatRepository repo, 
            IRagService ragService, 
            IPromptService promptService,
            IInferenceService inferenceService)
        {
            _repo = repo;
            _ragService = ragService;
            _promptService = promptService;
            _inferenceService = inferenceService;
        }

        public async Task<object> CreateAsync(CreateChatDto createChatDto)
        {
            var documents = await _ragService.RetrieveDocumentsAsync(
                createChatDto.Content);

            var response = await _inferenceService.GenerateResponseAsync(
                    createChatDto.Content, 
                    [],
                    documents);

            return await _repo.CreateAsync(createChatDto);
        }

        public async Task<object> GetAsync(string chatId)
        {
            var chat = await _repo.GetAsync(chatId);
            if (chat == null)
                throw new KeyNotFoundException($"Chat with id '{chatId}' not found.");
            return chat;
        }

        public async Task<object> UpdateAsync(string chatId, CreateChatItemDto createChatItemDto)
        {
            return await _repo.UpdateAsync(chatId, createChatItemDto);
        }

        public async Task DeleteAsync(string chatId)
        {
            await _repo.DeleteAsync(chatId);
        }
    }
}
