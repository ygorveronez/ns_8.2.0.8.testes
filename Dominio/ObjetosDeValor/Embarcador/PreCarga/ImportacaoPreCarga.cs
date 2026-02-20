using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.PreCarga
{
    public class ImportacaoPreCarga
    {
        public ImportacaoPreCarga()
        {
            this.Pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos { get; set; }
    }
}
