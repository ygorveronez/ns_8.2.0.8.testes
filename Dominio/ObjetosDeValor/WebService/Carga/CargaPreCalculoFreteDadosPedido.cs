using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class CargaPreCalculoFreteDadosPedido
    {
        [DataMember]
        public int ProtocoloPedido { get; set; }

        [DataMember]
        public decimal PesoCargaPedido { get; set; }
    }
}
