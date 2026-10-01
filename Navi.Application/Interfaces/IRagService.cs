using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navi.Domain.Entities;

namespace Navi.Application.Interfaces
{
    public interface IRagService
    {
        Task<IEnumerable<DocumentChunk>> SearchSimilarChunksAsync(float[] queryEmbedding, int topK = 3);
        Task SaveChunkAsync(DocumentChunk chunk);
    }
}
