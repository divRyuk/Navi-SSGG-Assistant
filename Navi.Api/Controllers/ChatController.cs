using Microsoft.AspNetCore.Mvc;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;

namespace Navi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IRagService _ragService;

    // Le pedimos a .NET que nos entregue TANTO el servicio de Chat COMO el del RAG
    public ChatController(IChatService chatService, IRagService ragService)
    {
        _chatService = chatService;
        _ragService = ragService;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SaveMessage([FromBody] ChatMessage message)
    {
        // 1. Guardamos el mensaje que envió el usuario
        await _chatService.SaveMessageAsync(message);

        // 2. Simulamos que convertimos la pregunta del usuario a un vector (coordenadas)
        float[] vectorSimulado = new float[] { 0.1f, 0.4f, 0.8f, 0.2f };

        // 3. Buscamos en el RAG si hay información útil sobre lo que preguntó
        var informacionRelevante = await _ragService.SearchSimilarChunksAsync(vectorSimulado, topK: 1);

        // 4. Respondemos confirmando que guardamos el mensaje Y mostrando qué machete encontró
        return Ok(new
        {
            status = "Mensaje procesado con éxito",
            mensajeGuardado = message.Text,
            macheteEncontrado = informacionRelevante
        });
    }

    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetHistory(string sessionId)
    {
        var history = await _chatService.GetChatHistoryAsync(sessionId);
        return Ok(history);
    }
}