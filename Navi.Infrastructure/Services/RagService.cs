using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;
using Navi.Infrastructure.Configurations;

namespace Navi.Infrastructure.Services;

public class RagService : IRagService
{
    private readonly IMongoCollection<DocumentChunk> _ragCollection;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILlmService _llmService;

    public RagService(
        IOptions<MongoDbSettings> settings,
        IEmbeddingService embeddingService,
        ILlmService llmService)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _ragCollection = database.GetCollection<DocumentChunk>(settings.Value.RagCollectionName);
        _embeddingService = embeddingService;
        _llmService = llmService;
    }

    public async Task SaveChunkAsync(DocumentChunk chunk)
    {
        await _ragCollection.InsertOneAsync(chunk);
    }

    public async Task<IEnumerable<DocumentChunk>> SearchSimilarChunksAsync(float[] queryEmbedding, int topK = 5)
    {
        var allChunks = await _ragCollection.Find(_ => true).ToListAsync();

        if (allChunks == null || !allChunks.Any())
            return Enumerable.Empty<DocumentChunk>();

        // Calculamos la similitud coseno y filtramos resultados irrelevantes (similitud > 0.35f)
        var rankedChunks = allChunks
            .Where(c => c.Embedding != null && c.Embedding.Length == queryEmbedding.Length)
            .Select(c => new
            {
                Chunk = c,
                Similarity = CosineSimilarity(queryEmbedding, c.Embedding!)
            })
            .Where(x => x.Similarity > 0.35f) // Umbral mínimo para descartar ruido
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToList();

        return rankedChunks;
    }

    /// <summary>
    /// Procesa la consulta del usuario mediante el flujo RAG completo:
    /// 1. Genera embedding de la pregunta con nomic-embed-text.
    /// 2. Recupera los chunks más similares de MongoDB.
    /// 3. Envía el contexto y la pregunta a llama3 para generar la respuesta final.
    /// </summary>
    public async Task<string> AskAsync(string question)
    {
        // 1. Obtener embedding de la pregunta
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(question);

        // 2. Buscar fragmentos similares en MongoDB (TopK = 5)
        var relevantChunks = await SearchSimilarChunksAsync(queryEmbedding.ToArray(), topK: 5);

        if (!relevantChunks.Any())
        {
            return "No encontré información relevante en los documentos procesados para responder a tu consulta.";
        }

        // 3. Formatear el contexto para el modelo de lenguaje
        string contextText = string.Join("\n\n---\n\n", relevantChunks.Select(c =>
            $"[Documento: {c.DocumentName} - {c.SectionTitle}]:\n{c.Content}"));

        string systemPrompt = $@"Sos Navi, un asistente virtual interno corporativo.
Respondé a la pregunta del usuario utilizando **únicamente** la siguiente información del procedimiento oficial.
Si la información no está en el texto, indicá que no poseés la información suficiente.

CONTEXTO OFICIAL:
{contextText}

PREGUNTA DEL USUARIO:
{question}";

        // 4. Generar respuesta final con llama3
        return await _llmService.GenerateResponseAsync(systemPrompt);
    }

    private float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        float dotProduct = 0.0f;
        float normA = 0.0f;
        float normB = 0.0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i]; // Corregido: vectorA por vectorB
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }

        if (normA == 0.0f || normB == 0.0f)
            return 0.0f;

        return dotProduct / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB));
    }
}