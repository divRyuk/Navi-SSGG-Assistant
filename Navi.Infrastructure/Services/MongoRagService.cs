using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;
using Navi.Infrastructure.Configurations;

namespace Navi.Infrastructure.Services;

public class MongoRagService : IRagService
{
    private readonly IMongoCollection<DocumentChunk> _ragCollection;

    public MongoRagService(IOptions<MongoDbSettings> settings)
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
        // Por ahora, simulamos trayendo los primeros 'topK' fragmentos guardados en MongoDB.
        // Más adelante conectaremos el índice de búsqueda de vectores (Vector Search).
        return await _ragCollection.Find(_ => true).Limit(topK).ToListAsync();
    }
}