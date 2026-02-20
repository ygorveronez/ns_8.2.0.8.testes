using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class RetornoCalculoFrete
    {
        public List<RetornoCalculoFreteValores> Valores { get; set; } = new List<RetornoCalculoFreteValores>();
    }
}
