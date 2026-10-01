using Microsoft.AspNetCore.Mvc;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;

namespace Navi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly IRagService _ragService;

    public RagController(IRagService ragService)
    {
        _ragService = ragService;
    }

    [HttpPost("chunk")]
    public async Task<IActionResult> SaveChunk([FromBody] DocumentChunk chunk)
    {
        await _ragService.SaveChunkAsync(chunk);
        return Ok(new { status = "Fragmento de documento guardado con éxito", chunkId = chunk.Id });
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchChunks([FromBody] float[] queryEmbedding, [FromQuery] int topK = 3)
    {
        var results = await _ragService.SearchSimilarChunksAsync(queryEmbedding, topK);
        return Ok(results);
    }
}