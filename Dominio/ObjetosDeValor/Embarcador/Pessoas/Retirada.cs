using MultiSoftware.NFe.v400.NotaFiscal;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    [DataContract]
    public class Retirada
    {
        [DataMember]
        public string CNPJ { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Logradouro { get; set; }

        [DataMember]
        public string Numero { get; set; }

        [DataMember]
        public string Bairro { get; set; }
        [DataMember]
        public string CodigoMunicipio { get; set; }

        [DataMember]
        public string Municipio { get; set; }

        [DataMember]
        public TUf UF { get; set; }

        [DataMember]
        public string CEP { get; set; }

        [DataMember]
        public Tpais CodigoPais { get; set; }

        [DataMember]
        public string Pais { get; set; }

        [DataMember]
        public string Telefone { get; set; }

        [DataMember]
        public string IE { get; set; }

    }
}
