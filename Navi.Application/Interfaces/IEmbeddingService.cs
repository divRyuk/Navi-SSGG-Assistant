using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navi.Application.Interfaces;

public interface IEmbeddingService
{
    /// <summary>
    /// Genera un vector numérico (embedding) a partir de un texto de entrada.
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string text);
}