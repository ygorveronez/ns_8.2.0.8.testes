using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class SimuladorFreteFrotaPedido
    {
        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoPedido> BlocoPedidos { get; set; }
    }
}
