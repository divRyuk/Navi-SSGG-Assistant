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
    private readonly ILlmService _llmService;

    public ChatController(IChatService chatService, IRagService ragService, ILlmService llmService)
    {
        _chatService = chatService;
        _ragService = ragService;
        _llmService = llmService;
    }

    [HttpPost("message")]
    public async Task<IActionResult> SaveMessage([FromBody] ChatMessage message)    
    {
        // 1. Guardamos el mensaje que envió el usuario
        await _chatService.SaveMessageAsync(message);

        // 2. Buscamos en el RAG la información relacionada (machete)
        float[] vectorSimulado = new float[] { 0.1f, 0.4f, 0.8f, 0.2f };
        var resultadosRag = await _ragService.SearchSimilarChunksAsync(vectorSimulado, topK: 1);

        string machete = resultadosRag.FirstOrDefault()?.Content ?? "";

        // 3. El LLM (Cerebro) genera la respuesta final redactada usando el machete
        string respuestaNavi = await _llmService.GenerateResponseAsync(message.Text, machete);

        // 4. Devolvemos la respuesta final procesada
        return Ok(new
        {
            status = "Procesado con éxito",
            preguntaUsuario = message.Text,
            respuestaNavi = respuestaNavi,
            fuenteUtilizada = resultadosRag.FirstOrDefault()?.DocumentName
        });
    }

    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetHistory(string sessionId)
    {
        var history = await _chatService.GetChatHistoryAsync(sessionId);
        return Ok(history);
    }
}