using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Amazon;
using System;
using System.Threading.Tasks;

namespace Lkp.Chat.Demo.Api.Services
{
    public interface IRagService
    {
        Task<List<KnowledgeBaseRetrievalResult>> RetrieveDocumentsAsync(string query);
    }
}
