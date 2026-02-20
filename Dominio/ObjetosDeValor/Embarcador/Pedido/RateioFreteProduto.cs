using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class RateioFreteProduto
    {
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string ChaveNFe { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroOcorrencia { get; set; }
        public decimal ValorRateado { get; set; }
        public decimal ValorRateadoICMS { get; set; }
        public decimal ValorRateadoFreteLiquido { get; set; }
        public decimal ValorRateadoValePedagio { get; set; }
        public List<RateioFreteProdutoComponentes> ListaRateioComponentes { get; set; }
    }
}
