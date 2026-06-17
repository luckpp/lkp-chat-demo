using Amazon;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Lkp.Chat.Demo.Api.Models;
using System;
using System.Threading.Tasks;

namespace Lkp.Chat.Demo.Api.Services
{
    public interface IRagService
    {
        Task<IEnumerable<RagDocument>> RetrieveDocumentsAsync(string query);
    }
}
