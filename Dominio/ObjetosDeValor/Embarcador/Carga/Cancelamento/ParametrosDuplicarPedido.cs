using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento
{
    public sealed class ParametrosDuplicarPedido
    {
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao> PedidoImportacoesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> PedidoProdutosAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> PedidoProdutosONUAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> PedidoTransbordosAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> PedidoComponentesFreteAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> PedidoAutorizacoesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> PedidoAnexosAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> PedidoAdicionalAntigos { get; set; }
    }
}
