using Lkp.Chat.Demo.Api.Models;

namespace Lkp.Chat.Demo.Api.Services.Implementation;

public class PromptService : IPromptService

{
    private readonly string _template = @"
You are a helpful AI assistant.

Your task is to respond to the user's latest message using the provided context and conversation history.

Rules:
- Use ONLY the information from the Context section when forming your answer.
- If the context does not contain relevant information, say: ""I don’t have enough information to answer this.""
- Be concise and clear.
- If the user message is a follow-up or continuation, use the Conversation History to understand intent.
- Do NOT invent facts or add external knowledge.
- Cite sources from the Context and include Source + Page.

Conversation History:
{0}

User Message:
{1}

Context:
{2}

Answer:
";

    public string BuildPrompt(string userInput, IEnumerable<RagDocument> ragDocuments, string chatHistory = "")
    {
        var context = string.Join("\n\n", ragDocuments.Select((d, i) =>
            $"[Doc{i + 1}]\nText: {d.Text}\nSource: {d.Location}, Page {d.PageNumber}"
        ));

        return string.Format(_template, chatHistory, userInput, context);
    }
}

