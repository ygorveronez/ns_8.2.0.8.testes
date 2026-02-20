using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class AjusteDataAgendamentoEntrega
    {
        [DataMember]
        public int ProtocoloPedido { get; set; }

        [DataMember]
        public string DataAgendamento { get; set; }
    }
}