using Microsoft.AspNetCore.Mvc;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;




namespace Navi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("message")]   
    public async Task<IActionResult> SaveMessage([FromBody] ChatMessage message)
    {
        await _chatService.SaveMessageAsync(message);
        return Ok(new { status = "Mensaje guardado correctamente", messageId = message.Id });
    }

    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetHistory(string sessionId)
    {
        var history = await _chatService.GetChatHistoryAsync(sessionId);
        return Ok(history);
    }
}