using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;
using Navi.Infrastructure.Configurations;

namespace Navi.Infrastructure.Services;

public class MongoChatService : IChatService
{
    private readonly IMongoCollection<ChatMessage> _chatCollection;

    public MongoChatService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _chatCollection = database.GetCollection<ChatMessage>(settings.Value.ChatCollectionName);
    }

    public async Task SaveMessageAsync(ChatMessage message)
    {
        // Guardamos el mensaje directamente en la base de datos MongoDB
        await _chatCollection.InsertOneAsync(message);
    }

    public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string sessionId)
    {
        // Buscamos todos los mensajes que pertenezcan a esa sesión y los ordenamos por fecha
        var filter = Builders<ChatMessage>.Filter.Eq(m => m.SessionId, sessionId);
        return await _chatCollection.Find(filter).SortBy(m => m.Timestamp).ToListAsync();
    }
}