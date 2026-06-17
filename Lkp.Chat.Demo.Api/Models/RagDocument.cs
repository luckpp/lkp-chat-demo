using Amazon.BedrockAgentRuntime.Model;

namespace Lkp.Chat.Demo.Api.Models;

public class RagDocument
{
    public string Text { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public double? Score { get; set; }

    public static RagDocument FromBedrockResult(KnowledgeBaseRetrievalResult result)
    {
        var pageNumber = 0;
        if (result.Metadata.TryGetValue("x-amz-bedrock-kb-document-page-number", out Amazon.Runtime.Documents.Document doc))
        {
            if (doc.Type == Amazon.Runtime.Documents.DocumentType.Double)
            {
                pageNumber = (int)doc.AsDouble();
            }
        }

        return new RagDocument
        {
            Text = result.Content.Text,
            Location = result.Location.S3Location.Uri,
            PageNumber = pageNumber,
            Score = result.Score
        };
    }
}
