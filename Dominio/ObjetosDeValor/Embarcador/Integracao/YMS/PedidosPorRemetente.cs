using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class PedidosPorRemetente
    {
        public string Remetente { get; set; }
        public string CnpjRemetente { get; set; }
        public List<int> Pedidos { get; set; }
    }
}
