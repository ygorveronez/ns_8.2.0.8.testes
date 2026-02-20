using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor
{
    [DataContract]
    public class Credencial
    {
        [DataMember]
        public string x { get; set; }

        [DataMember]
        public string y { get; set; }
    }
}
