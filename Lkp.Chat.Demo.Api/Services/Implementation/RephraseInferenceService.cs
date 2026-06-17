using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Lkp.Chat.Demo.Api.Dto;
using System.Text;
using System.Text.Json;

namespace Lkp.Chat.Demo.Api.Services.Implementation;

public class RephraseInferenceService : IRephraseInferenceService
{
    private const string SystemPrompt = @"
You are a query rephraser for a chat-based retrieval system.

Your task is to determine the user's intent from the latest message and the conversation history.

Rules:
- Return only the inferred user intent as a short search query.
- Preserve the meaning of the user's latest message.
- Resolve references from the conversation history, such as pronouns, follow-ups, and omitted nouns.
- Do not answer the user's question.
- Do not add explanations, punctuation, or quotes unless they are part of the intent.
";

    public async Task<string> DetermineUserIntentAsync(string userInput, IEnumerable<ChatItemDto> chatHistory)
    {
        var historyText = string.Join("\n", chatHistory.Select(item => $"{item.Role}: {item.Content}"));

        if (string.IsNullOrWhiteSpace(historyText))
        {
            return userInput;
        }

        var nativeRequest = JsonSerializer.Serialize(new
        {
            system = new[]
            {
                new { text = SystemPrompt }
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new[]
                    {
                        new
                        {
                            text = $"Conversation History:\n{historyText}\n\nLatest User Message:\n{userInput}\n\nReturn the inferred user intent as a search query."
                        }
                    }
                }
            },
            inferenceConfig = new
            {
                max_new_tokens = 64,
                temperature = 0.1,
                top_p = 0.9
            }
        });

        var client = new AmazonBedrockRuntimeClient();
        var modelId = "eu.amazon.nova-micro-v1:0";

        var request = new InvokeModelRequest
        {
            ModelId = modelId,
            Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
            ContentType = "application/json"
        };

        try
        {
            var response = await client.InvokeModelAsync(request);

            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();

            var jsonResponse = JsonDocument.Parse(responseBody);
            var output = jsonResponse.RootElement.GetProperty("output");
            var message = output.GetProperty("message");
            var content = message.GetProperty("content");

            if (content.GetArrayLength() > 0)
            {
                var text = content[0].GetProperty("text").GetString();
                return string.IsNullOrWhiteSpace(text) ? userInput : text.Trim();
            }

            return userInput;
        }
        catch (AmazonBedrockRuntimeException ex)
        {
            throw new InvalidOperationException($"Error determining user intent with model '{modelId}': {ex.Message}", ex);
        }
        finally
        {
            client?.Dispose();
        }
    }
}
