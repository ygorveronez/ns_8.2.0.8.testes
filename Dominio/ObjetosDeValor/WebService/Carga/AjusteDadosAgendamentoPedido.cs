using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class AjusteDadosAgendamentoPedido
    {
        [DataMember]
        public int ProtocoloPedido { get; set; }

        [DataMember]
        public string DataAgendamento { get; set; }

        [DataMember]
        public string SenhaAgendamentoCliente { get; set; }

        [DataMember]
        public string RestricaoEntrega { get; set; }
    }
}
