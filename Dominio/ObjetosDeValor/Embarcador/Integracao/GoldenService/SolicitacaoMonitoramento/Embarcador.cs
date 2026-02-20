using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    [XmlRoot(ElementName = "EMBARCADOR")]
    public class Embarcador
    {
        public Embarcador() { }

        public Embarcador(Dominio.Entidades.Cliente embarcador)
        {
            if (embarcador.Tipo == "F")
                CPF = embarcador.CPF_CNPJ_SemFormato;
            else
                CNPJ = embarcador.CPF_CNPJ_SemFormato;

            Nome = embarcador.Nome;
            //CEP = embarcador.CEP;
            //Rua = embarcador.Endereco;
            //Numero = embarcador.Numero;
            //Complemento = embarcador.Complemento;
            //Bairro = embarcador.Bairro;
            //Cidade = embarcador.Localidade?.Descricao;
            //UF = embarcador.Localidade?.Estado.Sigla;
        }

        [XmlElement(ElementName = "CPF")]
        public string CPF { get; set; }

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJ { get; set; }

        [XmlElement(ElementName = "NOME")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "CEP")]
        public string CEP { get; set; }

        [XmlElement(ElementName = "RUA")]
        public string Rua { get; set; }

        [XmlElement(ElementName = "NUM")]
        public string Numero { get; set; }

        [XmlElement(ElementName = "COMP")]
        public string Complemento { get; set; }

        [XmlElement(ElementName = "BAIRRO")]
        public string Bairro { get; set; }

        [XmlElement(ElementName = "CIDADE")]
        public string Cidade { get; set; }

        [XmlElement(ElementName = "UF")]
        public string UF { get; set; }
    }
}
