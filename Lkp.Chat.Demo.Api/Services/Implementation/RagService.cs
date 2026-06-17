using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using Lkp.Chat.Demo.Api.Models;
using Microsoft.Extensions.Options;

namespace Lkp.Chat.Demo.Api.Services.Implementation
{
    public class RagService : IRagService
    {
        private readonly IAmazonBedrockAgentRuntime _client;
        private readonly BedrockSettings _settings;

        public RagService(IAmazonBedrockAgentRuntime client, IOptions<BedrockSettings> settings)
        {
            _client = client;
            _settings = settings.Value;
        }

        public async Task<IEnumerable<RagDocument>> RetrieveDocumentsAsync(string query)
        {
            var ragResult = await RetrieveKnowledgeBaseDocumentsAsync(query);

            var result = ragResult
                .Select(RagDocument.FromBedrockResult)
                .ToList();

            return result;
        }

        private async Task<List<KnowledgeBaseRetrievalResult>> RetrieveKnowledgeBaseDocumentsAsync(string query)
        {
            try
            {
                var searchType = _settings.SearchType.Equals("Hybrid", StringComparison.OrdinalIgnoreCase)
                    ? SearchType.HYBRID
                    : SearchType.SEMANTIC;

                var request = new RetrieveRequest
                {
                    KnowledgeBaseId = _settings.KnowledgeBaseId,
                    RetrievalQuery = new KnowledgeBaseQuery { Text = query },
                    RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                    {
                        VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                        {
                            NumberOfResults = _settings.MaxResults,
                            OverrideSearchType = searchType
                        }
                    }
                };

                var response = await _client.RetrieveAsync(request);
                return response.RetrievalResults;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving documents: {ex.Message}", ex);
            }
        }
    }
}
