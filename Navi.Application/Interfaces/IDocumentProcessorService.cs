using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Application.Interfaces
{
    public interface IDocumentProcessorService
    {
        /// <summary>
        /// Recibe el flujo de datos (stream) de un archivo PDF y su nombre,
        /// extrae el texto, lo divide en fragmentos (chunks) y los devuelve listos para el RAG.
        /// </summary>
        Task<IEnumerable<Domain.Entities.DocumentChunk>> ProcessPdfAsync(Stream pdfStream, string fileName);
    }
}
