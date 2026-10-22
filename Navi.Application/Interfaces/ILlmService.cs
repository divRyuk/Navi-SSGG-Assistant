using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Application.Interfaces
{
    public interface ILlmService
    {
        /// <summary>
        /// Envía un prompt al Modelo de Lenguaje (LLM) y devuelve la respuesta generada.
        /// </summary>
        Task<string> GenerateResponseAsync(string userQuery, string context);
        Task<string> GenerateResponseAsync(string systemPrompt);
    }
}
