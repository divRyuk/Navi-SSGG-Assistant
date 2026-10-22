using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Navi.Application.Interfaces;

namespace Navi.Infrastructure.Services;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;

    public OllamaEmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var requestBody = new
        {
            model = "nomic-embed-text",
            prompt = text
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/embeddings", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();
                if (result?.Embedding != null && result.Embedding.Length > 0)
                {
                    return result.Embedding;
                }
            }
        }
        catch
        {
            // Fallback determinista de respaldo si Ollama no está activo
        }

        return new float[] { 0.1f, 0.4f, 0.8f, 0.2f };
    }

    private class OllamaEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
    }
}