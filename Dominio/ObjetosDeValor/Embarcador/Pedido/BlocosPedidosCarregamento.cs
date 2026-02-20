using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class BlocosPedidosCarregamento
    {
        public int ProtocoloPedido { get; set; }
        public int Carregamento { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.BlocoCarregamento> BlocosCarregamento { get; set; }
    }
}