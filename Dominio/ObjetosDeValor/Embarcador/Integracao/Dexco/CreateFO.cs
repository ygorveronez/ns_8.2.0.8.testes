using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Dexco
{
    public class CreateFO
    {
        public string AccessKey { get; set; }
        public string Fotype { get; set; }
        public List<FODelivery> FODeliveryList { get; set; }
        public int ProtocoloCarga { get; set; }
        public string NumeroCarga { get; set; }
        public string DataAgendamento { get; set; }
    }
}
