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

        public ChatService(IChatRepository repo, IRagService ragService, IPromptService promptService)
        {
            _repo = repo;
            _ragService = ragService;
            _promptService = promptService;
        }

        public async Task<object> CreateAsync(CreateChatDto createChatDto)
        {
            var ragResult = await _ragService.RetrieveDocumentsAsync(createChatDto.Content);
            
            var ragDocuments = ragResult
                .Select(r =>
                {
                    var pageNumber = 0;
                    if (r.Metadata.TryGetValue("x-amz-bedrock-kb-document-page-number", out Amazon.Runtime.Documents.Document doc))
                    {
                        if (doc.Type == Amazon.Runtime.Documents.DocumentType.Double)
                        {
                            pageNumber = (int) doc.AsDouble();
                            
                        }
                    }

                    return new RagDocument
                    {
                        Text = r.Content.Text,
                        LocationUri = r.Location.S3Location.Uri,
                        PageNumber = pageNumber,
                        Score = r.Score
                    };
                })
                .ToList();

            var prompt = _promptService.BuildPrompt(
                createChatDto.Content, 
                ragDocuments);

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
