using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Application.Interfaces
{
    public interface ILlmService
    {
        /// <summary>
        /// Toma la consulta del usuario y el contexto (machete del RAG)
        /// y genera la respuesta redactada de Navi.
        /// </summary>
        Task<string> GenerateResponseAsync(string userQuery, string context);
    }
}
