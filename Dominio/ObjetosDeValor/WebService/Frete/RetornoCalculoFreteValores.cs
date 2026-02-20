using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class RetornoCalculoFreteValores
    {
        public decimal ValorFreteLiquido { get; set; }

        public List<RetornoCalculoFreteValoresComponente> Componentes { get; set; }

        public decimal ValorTotalFrete { get; set; }
    }
}
