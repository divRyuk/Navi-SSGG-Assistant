using System.Text;
using System.Text.Json;
using Navi.Application.Interfaces;

namespace Navi.Infrastructure.Services;

public class OllamaLlmService : ILlmService
{
    private readonly HttpClient _httpClient;

    public OllamaLlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
    }

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        var requestBody = new
        {
            model = "llama3",
            prompt = prompt,
            stream = false,
            options = new
            {
                num_predict = 150, // Limita la respuesta a pocas oraciones directas
                num_ctx = 2048,    // Reduce la memoria de trabajo requerida por Ollama
                temperature = 0.2  // Respuesta rápida y centrada
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("/api/generate", jsonContent);

        if (!response.IsSuccessStatusCode)
        {
            return $"Error al comunicarse con Ollama: {response.StatusCode}";
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonResponse);

        if (doc.RootElement.TryGetProperty("response", out var responseElement))
        {
            return responseElement.GetString() ?? "Sin respuesta generada.";
        }

        return "No se pudo interpretar la respuesta del modelo.";
    }

    public async Task<string> GenerateResponseAsync(string prompt, string context)
    {
        string fullPrompt = $"Responde de forma muy breve y directa con el contexto brindado.\n\nContexto:\n{context}\n\nPregunta: {prompt}";
        return await GenerateResponseAsync(fullPrompt);
    }
}