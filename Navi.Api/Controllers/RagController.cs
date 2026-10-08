using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;

namespace Navi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly IRagService _ragService;
    private readonly IDocumentProcessorService _documentProcessor;
    private readonly IEmbeddingService _embeddingService;

    public RagController(
        IRagService ragService,
        IDocumentProcessorService documentProcessor,
        IEmbeddingService embeddingService)
    {
        _ragService = ragService;
        _documentProcessor = documentProcessor;
        _embeddingService = embeddingService;
    }

    [HttpPost("chunk")]
    public async Task<IActionResult> SaveChunk([FromBody] DocumentChunk chunk)
    {
        await _ragService.SaveChunkAsync(chunk);
        return Ok(new { status = "Chunk guardado correctamente en MongoDB", id = chunk.Id });
    }

    [HttpPost("upload-pdf")]
    public async Task<IActionResult> UploadPdf(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Por favor, seleccioná un archivo PDF válido.");

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("El archivo subido debe tener extensión .pdf.");

        using var stream = file.OpenReadStream();
        var chunks = await _documentProcessor.ProcessPdfAsync(stream, file.FileName);

        int totalChunks = 0;
        foreach (var chunk in chunks)
        {
            await _ragService.SaveChunkAsync(chunk);
            totalChunks++;
        }

        return Ok(new
        {
            status = "PDF procesado e ingestado exitosamente con Embeddings",
            fileName = file.FileName,
            totalChunksGuardados = totalChunks
        });
    }

    /// <summary>
    /// Realiza una búsqueda por similitud semántica (vectorial) según un texto.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int topK = 3)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Ingresá un texto de consulta.");

        // 1. Convertimos la consulta del usuario en un vector numérico (Embedding)
        var queryVector = await _embeddingService.GenerateEmbeddingAsync(query);

        // 2. Buscamos los chunks con mayor similitud coseno
        var resultados = await _ragService.SearchSimilarChunksAsync(queryVector, topK);

        return Ok(resultados);
    }
}