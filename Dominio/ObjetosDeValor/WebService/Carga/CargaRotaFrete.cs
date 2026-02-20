using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class CargaRotaFrete
    {
        [DataMember]
        public int ProtocoloCarga { get; set; }

        [DataMember]
        public string Polilinha { get; set; }

        [DataMember]
        public int TempoViagemMinutos { get; set; }

        [DataMember]
        public Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? TipoUltimoPontoRoteirizacao { get; set; }
    }
}
