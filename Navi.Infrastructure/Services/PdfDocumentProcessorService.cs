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

        using (var document = PdfDocument.Open(pdfStream))
        {
            foreach (var page in document.GetPages())
            {
                string pageText = page.Text;

                if (string.IsNullOrWhiteSpace(pageText))
                    continue;

                // Dividimos en bloques con solapamiento
                var pageChunks = SplitTextIntoChunks(pageText, chunkSizeInWords: 150, overlapInWords: 30);

                foreach (var chunkText in pageChunks)
                {
                    // Generamos el embedding REAL consumiendo Ollama
                    var embeddingVector = await _embeddingService.GenerateEmbeddingAsync(chunkText);

                    chunks.Add(new DocumentChunk
                    {
                        Id = Guid.NewGuid(),
                        DocumentName = fileName,
                        SectionTitle = $"Página {page.Number}",
                        PageNumber = page.Number,
                        Content = chunkText,
                        Embedding = embeddingVector.ToArray() // Vectores reales de nomic-embed-text
                    });
                }
            }
        }

        return chunks;
    }

    /// <summary>
    /// Fragmenta el texto en bloques de palabras manteniendo un solapamiento (overlap) 
    /// para evitar que las frases u oraciones importantes queden cortadas.
    /// </summary>
    private List<string> SplitTextIntoChunks(string text, int chunkSizeInWords, int overlapInWords)
    {
        var words = text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        if (words.Length == 0) return result;

        int step = chunkSizeInWords - overlapInWords;
        if (step <= 0) step = chunkSizeInWords; // Prevención de bucle infinito

        for (int i = 0; i < words.Length; i += step)
        {
            var chunkWords = words.Skip(i).Take(chunkSizeInWords);
            string chunkText = string.Join(" ", chunkWords).Trim();

            if (!string.IsNullOrWhiteSpace(chunkText))
            {
                result.Add(chunkText);
            }

            // Si ya llegamos al final del texto, salimos del bucle
            if (i + chunkSizeInWords >= words.Length)
                break;
        }

        return result;
    }
}