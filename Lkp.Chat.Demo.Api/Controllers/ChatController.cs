using Microsoft.AspNetCore.Mvc;
using Lkp.Chat.Demo.Api.Dto;
using Lkp.Chat.Demo.Api.Services;

namespace Lkp.Chat.Demo.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateChatDto createChatDto)
    {
        var chat = await _chatService.CreateAsync(createChatDto);
        return Ok(chat);
    }

    [HttpGet("{chatId}")]
    public async Task<IActionResult> Get(string chatId)
    {
        try
        {
            var chat = await _chatService.GetAsync(chatId);
            return Ok(chat);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{chatId}")]
    public async Task<IActionResult> Update(string chatId, CreateChatItemDto createChatItemDto)
    {
        try
        {
            var item = await _chatService.UpdateAsync(chatId, createChatItemDto);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{chatId}")]
    public async Task<IActionResult> Delete(string chatId)
    {
        await _chatService.DeleteAsync(chatId);
        return NoContent();
    }
}