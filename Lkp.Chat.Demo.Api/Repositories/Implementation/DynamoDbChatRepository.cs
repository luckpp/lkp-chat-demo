using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Lkp.Chat.Demo.Api.Dto;

namespace Lkp.Chat.Demo.Api.Repositories.Implementation;

public class DynamoDbChatRepository : IChatRepository
{
    private readonly IAmazonDynamoDB _dynamo;
    private const string TableName = "Chats";

    public DynamoDbChatRepository(IAmazonDynamoDB dynamo)
    {
        _dynamo = dynamo;
    }

    public async Task<ChatDto> CreateAsync(CreateChatDto request)
    {
        var chatId = Guid.NewGuid().ToString();
        var itemId = Guid.NewGuid().ToString();

        await _dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["ChatId"] = new AttributeValue(chatId),
                ["Name"] = new AttributeValue(request.Name),
                ["Items"] = new AttributeValue
                {
                    L =
                    [
                        new AttributeValue
                        {
                            M = new Dictionary<string, AttributeValue>
                            {
                                ["Id"] = new AttributeValue(itemId),
                                ["Content"] = new AttributeValue(request.Content)
                            }
                        }
                    ]
                }
            }
        });

        return new ChatDto
        {
            Id = chatId,
            Name = request.Name,
            Items =
            [
                new ChatItemDto
                {
                    Id = itemId,
                    Content = request.Content
                }
            ]
        };
    }

    public async Task<ChatDto?> GetAsync(string chatId)
    {
        var response = await _dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["ChatId"] = new AttributeValue(chatId)
            }
        });

        if (!response.Item.Any()) return null;

        var items = new List<ChatItemDto>();

        if (response.Item.TryGetValue("Items", out var itemsAttribute) && itemsAttribute.L is { Count: > 0 })
        {
            items = itemsAttribute.L
                .Where(a => a.M is { Count: > 0 })
                .Select(a => new ChatItemDto
                {
                    Id = a.M.TryGetValue("Id", out var id) ? id.S : string.Empty,
                    Content = a.M.TryGetValue("Content", out var content) ? content.S : string.Empty
                })
                .ToList();
        }

        return new ChatDto
        {
            Id = response.Item["ChatId"].S,
            Name = response.Item.TryGetValue("Name", out var name) ? name.S : string.Empty,
            Items = items
        };
    }

    public async Task<ChatItemDto> UpdateAsync(string chatId, CreateChatItemDto request)
    {
        var itemId = Guid.NewGuid().ToString();

        try
        {
            await _dynamo.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["ChatId"] = new AttributeValue(chatId)
                },
                ConditionExpression = "attribute_exists(ChatId)",
                UpdateExpression = "SET Items = list_append(if_not_exists(Items, :emptyList), :newItem)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":emptyList"] = new AttributeValue { L = [] },
                    [":newItem"] = new AttributeValue
                    {
                        L =
                        [
                            new AttributeValue
                            {
                                M = new Dictionary<string, AttributeValue>
                                {
                                    ["Id"] = new AttributeValue(itemId),
                                    ["Content"] = new AttributeValue(request.Content)
                                }
                            }
                        ]
                    }
                }
            });
        }
        catch (ConditionalCheckFailedException)
        {
            throw new KeyNotFoundException($"Chat '{chatId}' was not found.");
        }

        return new ChatItemDto
        {
            Id = itemId,
            Content = request.Content
        };
    }

    public async Task DeleteAsync(string chatId)
    {
        await _dynamo.DeleteItemAsync(new DeleteItemRequest
        {
            TableName = TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["ChatId"] = new AttributeValue(chatId)
            }
        });
    }
}
