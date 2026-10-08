using System.Text;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;
using UglyToad.PdfPig;

namespace Navi.Infrastructure.Services;

public class PdfDocumentProcessorService : IDocumentProcessorService
{
    private readonly IEmbeddingService _embeddingService;

    public PdfDocumentProcessorService(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }
    public async Task<IEnumerable<DocumentChunk>> ProcessPdfAsync(Stream pdfStream, string fileName)
    {

        var chunks = new List<DocumentChunk>();

        // Abrimos el PDF desde el stream que nos envía la API
        using (var document = PdfDocument.Open(pdfStream))
        {
            foreach (var page in document.GetPages())
            {
                string pageText = page.Text;

                if (string.IsNullOrWhiteSpace(pageText))
                    continue;

                // Dividimos el texto de la página en fragmentos de aprox. 300 palabras
                var pageChunks = SplitTextIntoChunks(pageText, maxWordsPerChunk: 300);

                foreach (var chunkText in pageChunks)
                {
                    chunks.Add(new DocumentChunk
                    {
                        Id = Guid.NewGuid(),
                        DocumentName = fileName,
                        SectionTitle = $"Página {page.Number}",
                        PageNumber = page.Number,
                        Content = chunkText,
                        // Por ahora usamos el vector simulado hasta conectar el generador de Embeddings
                        Embedding = new float[] { 0.1f, 0.4f, 0.8f, 0.2f }
                    });
                }
            }
        }

        return chunks;
    }

    /// <summary>
    /// Función auxiliar para cortar un texto largo en bloques de palabras.
    /// </summary>
    private List<string> SplitTextIntoChunks(string text, int maxWordsPerChunk)
    {
        var words = text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();
        var currentChunk = new StringBuilder();
        int wordCount = 0;

        foreach (var word in words)
        {
            currentChunk.Append(word).Append(" ");
            wordCount++;

            if (wordCount >= maxWordsPerChunk)
            {
                result.Add(currentChunk.ToString().Trim());
                currentChunk.Clear();
                wordCount = 0;
            }
        }

        if (currentChunk.Length > 0)
        {
            result.Add(currentChunk.ToString().Trim());
        }

        return result;
    }
}