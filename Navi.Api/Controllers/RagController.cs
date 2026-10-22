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
    private readonly ILlmService _llmService;

    public RagController(
        IRagService ragService,
        IDocumentProcessorService documentProcessor,
        IEmbeddingService embeddingService,
        ILlmService llmService)
    {
        _ragService = ragService;
        _documentProcessor = documentProcessor;
        _embeddingService = embeddingService;
        _llmService = llmService;
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

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int topK = 5)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Ingresá un texto de consulta.");

        var queryVector = await _embeddingService.GenerateEmbeddingAsync(query);
        var resultados = await _ragService.SearchSimilarChunksAsync(queryVector, topK);

        return Ok(resultados);
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] string query, [FromQuery] int topK = 2)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Ingresá una pregunta válida.");

        var queryVector = await _embeddingService.GenerateEmbeddingAsync(query);
        var chunks = await _ragService.SearchSimilarChunksAsync(queryVector, topK);

        if (!chunks.Any())
        {
            return Ok(new
            {
                question = query,
                answer = "No encontré información relevante en los documentos procesados para responder a tu pregunta."
            });
        }

        string context = string.Join("\n\n", chunks.Select(c => $"[{c.DocumentName} - {c.SectionTitle}]: {c.Content}"));

        string prompt = $"Respondé en 2 oraciones basándote solo en esto:\n\n{context}\n\nPregunta: {query}";
        string response = await _llmService.GenerateResponseAsync(prompt);

        return Ok(new
        {
            question = query,
            answer = response,
            sourcesUsed = chunks.Select(c => new { c.DocumentName, c.SectionTitle, c.PageNumber })
        });
    }
}