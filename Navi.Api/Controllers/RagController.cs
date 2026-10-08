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

    public RagController(IRagService ragService, IDocumentProcessorService documentProcessor)
    {
        _ragService = ragService;
        _documentProcessor = documentProcessor;
    }

    /// <summary>
    /// Permite guardar un fragmento individual a mano.
    /// </summary>
    [HttpPost("chunk")]
    public async Task<IActionResult> SaveChunk([FromBody] DocumentChunk chunk)
    {
        await _ragService.SaveChunkAsync(chunk);
        return Ok(new { status = "Chunk guardado correctamente en MongoDB", id = chunk.Id });
    }

    /// <summary>
    /// Permite subir un archivo PDF real, extraer su texto, cortarlo en chunks
    /// y guardarlos automáticamente en MongoDB.
    /// </summary>
    [HttpPost("upload-pdf")]
    public async Task<IActionResult> UploadPdf(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Por favor, seleccioná un archivo PDF válido.");

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("El archivo subido debe tener extensión .pdf.");

        // 1. Procesamos el PDF y lo cortamos en fragmentos (chunks)
        using var stream = file.OpenReadStream();
        var chunks = await _documentProcessor.ProcessPdfAsync(stream, file.FileName);

        int totalChunks = 0;
        // 2. Guardamos cada fragmento en MongoDB
        foreach (var chunk in chunks)
        {
            await _ragService.SaveChunkAsync(chunk);
            totalChunks++;
        }

        return Ok(new
        {
            status = "PDF procesado e ingestado exitosamente",
            fileName = file.FileName,
            totalChunksGuardados = totalChunks
        });
    }

    /// <summary>
    /// Realiza una búsqueda de similitud simulada.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] int topK = 3)
    {
        float[] vectorSimulado = new float[] { 0.1f, 0.4f, 0.8f, 0.2f };
        var resultados = await _ragService.SearchSimilarChunksAsync(vectorSimulado, topK);
        return Ok(resultados);
    }
}