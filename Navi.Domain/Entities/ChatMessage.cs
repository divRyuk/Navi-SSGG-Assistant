using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SessionId { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty; // "User" o "Navi"
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? CitationSource { get; set; } // Ej: "Punto 5.8 de LI-ADM-CC-04"
    }
}
