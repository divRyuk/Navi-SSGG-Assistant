using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;
using Navi.Infrastructure.Configurations;

namespace Navi.Infrastructure.Services;

public class RagService : IRagService
{
    private readonly IMongoCollection<DocumentChunk> _ragCollection;

    public RagService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _ragCollection = database.GetCollection<DocumentChunk>(settings.Value.RagCollectionName);
    }

    public async Task SaveChunkAsync(DocumentChunk chunk)
    {
        // Guardamos el fragmento del manual en la base de datos
        await _ragCollection.InsertOneAsync(chunk);
    }

    public async Task<IEnumerable<DocumentChunk>> SearchSimilarChunksAsync(float[] queryEmbedding, int topK = 3)
    {
        // 1. Obtenemos todos los chunks almacenados en MongoDB
        var allChunks = await _ragCollection.Find(_ => true).ToListAsync();

        if (allChunks == null || !allChunks.Any())
            return Enumerable.Empty<DocumentChunk>();

        // 2. Calculamos la similitud coseno entre el embedding de la consulta y los de los chunks
        var rankedChunks = allChunks
            .Where(c => c.Embedding != null && c.Embedding.Length == queryEmbedding.Length)
            .Select(c => new
            {
                Chunk = c,
                Similarity = CosineSimilarity(queryEmbedding, c.Embedding!)
            })
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.Chunk);

        return rankedChunks;
    }

    /// <summary>
    /// Computa la similitud coseno entre dos vectores (Embeddings).
    /// Resultado cercano a 1.0 = Máxima similitud de contenido.
    /// </summary>
    private float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        float dotProduct = 0.0f;
        float normA = 0.0f;
        float normB = 0.0f;

        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }

        if (normA == 0.0f || normB == 0.0f)
            return 0.0f;

        return dotProduct / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB));
    }
}