using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Eventos
    {
        public string flow { get; set; }
        public bool hasDeliveryReceipt { get; set; }
        public List<Evento> items { get; set; }
    }
}
