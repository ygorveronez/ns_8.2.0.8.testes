using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Localidade
{
    public class Endereco
    {
        [DataMember]
        public Dominio.ObjetosDeValor.Localidade Cidade { get; set; }

        [DataMember]
        public string Logradouro { get; set; }

        [DataMember]
        public string Numero { get; set; }

        [DataMember]
        public string Complemento { get; set; }

        [DataMember]
        public string CEP { get; set; }

        [DataMember]
        public string CEPSemFormato { get; set; }

        [DataMember]
        public string Bairro { get; set; }

        [DataMember]
        public string DDDTelefone { get; set; }

        [DataMember]
        public string Telefone { get; set; }

        [DataMember]
        public string Telefone2 { get; set; }

        [DataMember]
        public string CodigoIntegracao { get; set; }

        [DataMember]
        public string InscricaoEstadual { get; set; }

        [DataMember]
        public string Latitude { get; set; }

        [DataMember]
        public string Longitude { get; set; }

        [DataMember]
        public string UF { get; set; }

        [DataMember]
        public string CodigoTelefonicoPais { get; set; }

        public string EnderecoConcatenado { get; set; }

        public EnderecoDadosComplementares DadosComplementares { get; set; }

        public Dominio.ObjetosDeValor.Localidade Estado { get; set; }
    }
}
