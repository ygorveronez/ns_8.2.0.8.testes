using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class AgendamentoColeta
    {
        public int Codigo { get; set; }
        public DateTime? DataEntrega { get; set; }
        public string TransportadorManual { get; set; }
        public int CodigoCarga { get; set; }
    }
}
