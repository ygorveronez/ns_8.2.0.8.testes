using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Dominio.ObjetosDeValor.WebService.Carga
{

    [DataContract]
    public class CargaPreCalculoFrete
    {

        [DataMember]
        public int ProtocoloCarga { get; set; }

        [DataMember]
        public string ProtocoloTipoOperacao { get; set; }

        [DataMember]
        public List<CargaPreCalculoFreteDadosPedido> DadosPedidos { get; set; }

    }

}
