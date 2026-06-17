using Amazon;
using Amazon.Bedrock;
using Amazon.Bedrock.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using System.Text;
using System.Text.Json;

namespace Lkp.Chat.Demo.Api.Services.Implementation;

public class InferenceService : IInferenceService
{

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        // Create a Bedrock Runtime client
        var client = new AmazonBedrockRuntimeClient(RegionEndpoint.USEast1);

        // Use Amazon Nova Micro - most cost-effective option
        var modelId = "amazon.nova-micro-v1:0";

        // Format the request payload for Nova models
        var nativeRequest = JsonSerializer.Serialize(new
        {
            messages = new[]
            {
                new { role = "user", content = new[] { new { text = prompt } } }
            },
            inferenceConfig = new
            {
                max_new_tokens = 512,
                temperature = 0.7,
                top_p = 0.9
            }
        });

        // Create the request
        var request = new InvokeModelRequest()
        {
            ModelId = modelId,
            Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
            ContentType = "application/json"
        };

        try
        {
            // Send the request to Bedrock Runtime
            var response = await client.InvokeModelAsync(request);

            // Read and parse the response
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
