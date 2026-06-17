namespace Lkp.Chat.Demo.Api.Models;

public class RagDocument
{
    public string Text { get; set; } = string.Empty;
    public string LocationUri { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public double? Score { get; set; } 
}
