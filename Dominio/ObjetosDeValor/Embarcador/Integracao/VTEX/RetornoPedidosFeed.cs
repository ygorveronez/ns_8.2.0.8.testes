using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX
{
    public class RetornoPedidosFeed
    {
        public string eventId { get; set; }
        public string handle { get; set; }
        public string domain { get; set; }
        public string state { get; set; }
        public string lastState { get; set; }
        public string orderId { get; set; }
        public DateTime? lastChange { get; set; }
        public DateTime? currentChange { get; set; }
        public DateTime? availableDate { get; set; }
    }

    public class handlesEnvio
    {
        public List<string> handles { get; set; }
    }

}
