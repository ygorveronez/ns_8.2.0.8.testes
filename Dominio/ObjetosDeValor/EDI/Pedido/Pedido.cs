using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Pedido
{
    public class Pedido
    {
        public CabecalhoDocumento CabecalhoDocumento { get; set; }
        public List<DetalhePedido> Pedidos { get; set; }
        public RodapeDocumento RodapeDocumento { get; set; }
    }
}
