using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class CarregamentoConsinco
    {
        public int numeroCargaTMS { get; set; }

        public string modeloveicular { get; set; }

        public string usuarioCarga { get; set; }

        public List<PedidoConsinco> pedidos { get; set; }
    }
}
