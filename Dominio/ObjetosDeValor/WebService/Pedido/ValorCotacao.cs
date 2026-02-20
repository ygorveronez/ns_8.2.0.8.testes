using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public class ValorCotacao
    {
        public decimal FreteProprio { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente> Componentes { get; set; }
        public decimal ValorTotalCotacao { get; set; }
        public decimal Valor { get; set; }
        public decimal BaseCalculo { get; set; }
        public decimal Aliquota { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorISSRetido { get; set; }
    }
}
