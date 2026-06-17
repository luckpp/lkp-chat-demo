using Amazon;
using Amazon.Bedrock;
using Amazon.Bedrock.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Lkp.Chat.Demo.Api.Dto;
using Lkp.Chat.Demo.Api.Models;
using System.Text;
using System.Text.Json;

namespace Lkp.Chat.Demo.Api.Services.Implementation;

public class InferenceService : IInferenceService
{
    private readonly string SystemPrompt = @"
You are a helpful AI assistant.

Your task is to respond to the user's latest message using the provided context and conversation history.

Rules:
- Use ONLY the information from the Context section when forming your answer.
- If the context does not contain relevant information, say: ""I don’t have enough information to answer this.""
- Be concise and clear.
- If the user message is a follow-up or continuation, use the Conversation History to understand intent.
- Do NOT invent facts or add external knowledge.
- Cite sources from the Context and include Source + Page.
";

    public async Task<string> GenerateResponseAsync(
        string userInput,
        IEnumerable<ChatItemDto> history,
        IEnumerable<RagDocument> context)
    {
        await ListModelsAsync();

        var contextText = string.Join("\n\n",
            context.Select((d, i) =>
                $"[Doc{i + 1}]\nText: {d.Text}\nSource: {d.Location}, Page {d.PageNumber}"
            ));

        // 2. Build messages (history + latest user input)
        var messages = new List<object>();

        // Add history
        foreach (var item in history)
        {
            messages.Add(new
            {
                role = item.Role,
                content = new[]
                {
                    new { text = item.Content }
                }
            });
        }

        // Add latest user message (with context)
        messages.Add(new
        {
            role = "user",
            content = new[]
            {
                new
                {
                    text = $@"Context:
{contextText}

User Message:
{userInput}"
                }
            }
        });

        var nativeRequest = JsonSerializer.Serialize(new
        {
            system = new[]
            {
                new { text = SystemPrompt }
            },
            messages,
            inferenceConfig = new
            {
                max_new_tokens = 512,
                temperature = 0.7,
                top_p = 0.9
            }
        });

        var client = new AmazonBedrockRuntimeClient();

        var modelId = "eu.amazon.nova-micro-v1:0";

        var request = new InvokeModelRequest()
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
                return text ?? string.Empty;
            }

            return string.Empty;
        }
        catch (AmazonBedrockRuntimeException ex)
        {
            throw new InvalidOperationException($"Error invoking Bedrock model '{modelId}': {ex.Message}", ex);
        }
        finally
        {
            client?.Dispose();
        }
    }


    private async Task ListModelsAsync()
    {
        var bedrockClient = new AmazonBedrockClient();
        var modelsResponse = await bedrockClient.ListFoundationModelsAsync(new ListFoundationModelsRequest());

        Console.WriteLine("Available models:");
        foreach (var model in modelsResponse.ModelSummaries)
        {
            Console.WriteLine($"- {model.ModelId}");
        }
    }


    //public async Task<string> GenerateResponseAsync(string prompt)
    //{
    //    var client = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);

    //    var modelId = "amazon.nova-micro-v1:0";

    //    var nativeRequest = JsonSerializer.Serialize(new
    //    {
    //        messages = new[]
    //        {
    //            new { role = "user", content = new[] { new { text = prompt } } }
    //        },
    //        inferenceConfig = new
    //        {
    //            max_new_tokens = 512,
    //            temperature = 0.7,
    //            top_p = 0.9
    //        }
    //    });

    //    // Create the request
    //    var request = new InvokeModelRequest()
    //    {
    //        ModelId = modelId,
    //        Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
    //        ContentType = "application/json"
    //    };

    //    try
    //    {
    //        // Send the request to Bedrock Runtime
    //        var response = await client.InvokeModelAsync(request);

    //        // Read and parse the response
    //        using var reader = new StreamReader(response.Body);
    //        var responseBody = await reader.ReadToEndAsync();

    //        var jsonResponse = JsonDocument.Parse(responseBody);
    //        var output = jsonResponse.RootElement.GetProperty("output");
    //        var message = output.GetProperty("message");
    //        var content = message.GetProperty("content");

    //        if (content.GetArrayLength() > 0)
    //        {
    //            var text = content[0].GetProperty("text").GetString();
    //            return text ?? string.Empty;
    //        }

    //        return string.Empty;
    //    }
    //    catch (AmazonBedrockRuntimeException ex)
    //    {
    //        throw new InvalidOperationException($"Error invoking Bedrock model '{modelId}': {ex.Message}", ex);
    //    }
    //    finally
    //    {
    //        client?.Dispose();
    //    }
    //}


}

//public async Task<string> GenerateResponseAsync(string prompt)
//{
//    var bedrockClient = new AmazonBedrockClient();
//    var modelsResponse = await bedrockClient.ListFoundationModelsAsync(new ListFoundationModelsRequest());

//    Console.WriteLine("Available models:");
//    foreach (var model in modelsResponse.ModelSummaries)
//    {
//        Console.WriteLine($"- {model.ModelId}");
//    }





//    var client = new AmazonBedrockRuntimeClient();
//    var modelId = "amazon.nova-lite-v1:0";

//    var nativeRequest = JsonSerializer.Serialize(new
//    {
//        inputText = prompt,
//        textGenerationConfig = new
//        {
//            maxTokenCount = 512,
//            temperature = 0.7,
//            topP = 0.9,
//            stopSequences = new string[] { }
//        }
//    });

//    var request = new InvokeModelRequest()
//    {
//        ModelId = modelId,
//        Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
//        ContentType = "application/json"
//    };

//    try
//    {
//        var response = await client.InvokeModelAsync(request);

//        using var reader = new StreamReader(response.Body);
//        var responseBody = await reader.ReadToEndAsync();

//        var jsonResponse = JsonDocument.Parse(responseBody);
//        var results = jsonResponse.RootElement.GetProperty("results");

//        if (results.GetArrayLength() > 0)
//        {
//            var outputText = results[0].GetProperty("outputText").GetString();
//            return outputText ?? string.Empty;
//        }

//        return string.Empty;
//    }
//    catch (AmazonBedrockRuntimeException ex)
//    {
//        throw new InvalidOperationException($"Error invoking Bedrock model '{modelId}': {ex.Message}", ex);
//    }
//    finally
//    {
//        client?.Dispose();
//    }
//}
