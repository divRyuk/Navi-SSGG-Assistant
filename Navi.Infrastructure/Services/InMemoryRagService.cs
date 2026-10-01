using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navi.Application.Interfaces;
using Navi.Domain.Entities;

namespace Navi.Infrastructure.Services
{
    public class InMemoryRagService : IRagService
    {
        private readonly List<DocumentChunk> _chunks = new();

        public Task SaveChunkAsync(DocumentChunk chunk)
        {
            _chunks.Add(chunk);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<DocumentChunk>> SearchSimilarChunksAsync(float[] queryEmbedding, int topK = 3)
        {
            // Por ahora devuelve los primeros K fragmentos guardados
            var result = _chunks.Take(topK);
            return Task.FromResult(result);
        }
    }
}
