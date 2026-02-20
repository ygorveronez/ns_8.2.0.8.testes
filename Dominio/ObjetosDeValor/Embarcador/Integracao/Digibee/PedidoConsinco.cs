using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class PedidoConsinco
    {
        public int codigoOrigem { get; set; }
        public int codigoDestino { get; set; }
        public int numeroPedidoTMS { get; set; }
        public int numeroPedidoERP { get; set; }
        public List<Digibee.ProdutoConsinco> itens { get; set; }
    }
}
