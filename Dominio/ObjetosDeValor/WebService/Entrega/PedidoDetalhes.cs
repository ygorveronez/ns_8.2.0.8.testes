using System;

namespace Dominio.ObjetosDeValor.WebService.Entrega
{
    public class PedidoDetalhes
    {
        public int NumeroPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int ProtocoloPedido { get; set; }
        public DateTime? DataPrevisaoEntrega { get; set; }
        public decimal? ValorNFs { get; set; }
        public string Vendedor { get; set; }
        public string NotasFiscais { get; set; }

    }
}
