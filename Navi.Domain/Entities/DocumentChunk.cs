using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Domain.Entities
{
    public class DocumentChunk
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DocumentName { get; set; } = string.Empty; // Ej: "LI-ADM-CC-04.pdf"
        public string SectionTitle { get; set; } = string.Empty; // Ej: "5.8 Envíos Logísticos"
        public int PageNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public float[]? Embedding { get; set; } // Representación vectorial del texto
    }
}
