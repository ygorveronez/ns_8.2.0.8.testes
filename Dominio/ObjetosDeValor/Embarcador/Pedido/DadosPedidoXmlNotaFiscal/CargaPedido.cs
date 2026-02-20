using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class CargaPedido
    {
        public int Codigo { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public Pedido Pedido { get; set; }

        public Cliente Expedidor { get; set; }

        public Cliente Recebedor { get; set; }

        public List<CargaPedidoProduto> Produtos { get; set; }
    }
}
