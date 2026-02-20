using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.GTA
{
    public class Pedido
    {
        public int Numero { get; set; }
        public List<SuinoPorBox> SuinosPorBox { get; set; }
        public int ImunoRealizado { get; set; }
        public int ImunoPlanejado { get; set; }
        public int NumeroProtocoloIntegracaoPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
    }
}