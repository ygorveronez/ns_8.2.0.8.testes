using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    [DataContract]
    public class Componente
    {
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public string CodigoIntegracao { get; set; }
        
        public int Codigo { get; set; }
    }
}
