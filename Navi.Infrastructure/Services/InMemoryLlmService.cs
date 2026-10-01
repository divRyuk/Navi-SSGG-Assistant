using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navi.Application.Interfaces;

namespace Navi.Infrastructure.Services
{
    public class InMemoryLlmService : ILlmService 
    {
        public Task<string> GenerateResponseAsync(string userQuery, string context)
        {
            // Si no se encontró nada en el manual, Navi responde amablemente que no tiene la información
            if (string.IsNullOrWhiteSpace(context))
            {
                return Task.FromResult("Hola. Lo siento, no encontré información sobre esa consulta en el manual de Servicios Generales. ¿Deseás que registre un ticket con el área?");
            }

            // Si hay contexto/machete, Navi simula la redacción inteligente combinando los datos
            string respuestaRedactada = $"¡Hola! Con gusto te ayudo. Según el procedimiento oficial de Servicios Generales:\n\n" +
                                       $"\"{context}\"\n\n" +
                                       $"Si necesitás tramitar esto o tenés alguna otra duda, avisame y te guío con el proceso.";

            return Task.FromResult(respuestaRedactada);
        }
    }
}
