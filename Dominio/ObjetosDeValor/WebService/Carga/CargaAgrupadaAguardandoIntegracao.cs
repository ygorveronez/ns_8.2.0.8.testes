using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class CargaAgrupadaAguardandoIntegracao
    {
        [DataMember]
        public int ProtocoloCarga { get; set; }

        [DataMember]
        public List<CargaFilhaAguardandoIntegracao> CargasFilhas { get; set; }
        
    }

    public class CargaFilhaAguardandoIntegracao
    {
        [DataMember]
        public int ProtocoloCarga { get; set; }
    }



    public class ProtocolosAguardandoIntegracao
    {
        public int ProtocoloCargaPai { get; set; }
        public int ProtocoloCargaFilha { get; set; }
    }



}
