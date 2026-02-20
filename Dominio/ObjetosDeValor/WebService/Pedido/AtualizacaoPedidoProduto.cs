using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public class AtualizacaoPedidoProduto
    {
        public int ProtocoloIntegracaoPedido { get; set; }

        public List<Embarcador.Pedido.Produto> Produtos { get; set; }
    }
}
