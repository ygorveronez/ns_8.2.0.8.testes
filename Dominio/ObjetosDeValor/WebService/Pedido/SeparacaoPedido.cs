using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class SeparacaoPedido
    {
        public int ProtocoloIntegracaoPedido { get; set; }

        public int PercentualSeparacao { get; set; }

        public bool PedidoBloqueado { get; set; }

        public bool PermitirLiberarPedido { get; set; }

        public List<SeparacaoPedidoProduto> Produtos { get; set; }

        public bool? PedidoRestricaoData { get; set; }
    }
}
