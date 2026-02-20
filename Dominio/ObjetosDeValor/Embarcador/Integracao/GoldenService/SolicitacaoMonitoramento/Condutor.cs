using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    public class Condutor
    {
        public Condutor() { }

        public Condutor(Dominio.Entidades.Usuario condutor)
        {
            CPF = condutor.CPF;
            Nome = condutor.Nome;
            Tipo = condutor.TipoMotorista == Enumeradores.TipoMotorista.Proprio ? "1" : "2";
            Sexo = condutor.Sexo == Enumerador.Sexo.Feminino ? "F" : "M";
            CNH = condutor.NumeroHabilitacao ?? string.Empty;
            UFCNH = condutor.Localidade?.Estado.Sigla ?? string.Empty;
            Categoria = condutor.Categoria ?? string.Empty;
            DataValidadeCNH = condutor.DataVencimentoHabilitacao?.ToString("ddMMyyyy") ?? string.Empty;
            //RG = condutor.RG;
            //UFRG = condutor.EstadoRG?.Sigla;
            //OrgaoEmissorRG = condutor.OrgaoEmissorRG.HasValue ? condutor.OrgaoEmissorRG?.ToString("G") + "/" + condutor.EstadoRG?.Sigla : null;
            //DataEmissaoRG = ;
            //DataNascimento = condutor.DataNascimento?.ToString("ddMMyyyy");
            //CidadeNascimento = condutor.Localidade?.Descricao;
            //UFNascimento = condutor.Localidade?.Estado.Sigla;
            //NomePai = ;
            //NomeMae = ;
            //CEP = condutor.CEP;
            //Rua = condutor.Endereco;
            //Numero = condutor.NumeroEndereco ;
            //Complemento = condutor.Complemento;
            //Bairro = condutor.Bairro;
            //Cidade = condutor.Localidade?.Descricao;
            //UF = condutor.Localidade?.Estado.Sigla;
            Celular1 = Utilidades.String.OnlyNumbers(condutor.Telefone) ?? string.Empty;
            //Celular2 = ;
            //Telefone1 = ;
            //Telefone2 = ;
            //IdNextel = ;
        }

        [XmlElement(ElementName = "CPF")]
        public string CPF { get; set; }

        [XmlElement(ElementName = "NOME")]
        public string Nome { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "SEXO")]
        public string Sexo { get; set; }

        [XmlElement(ElementName = "CNH")]
        public string CNH { get; set; }

        [XmlElement(ElementName = "UFCNH")]
        public string UFCNH { get; set; }

        [XmlElement(ElementName = "CAT")]
        public string Categoria { get; set; }

        [XmlElement(ElementName = "VALID")]
        public string DataValidadeCNH { get; set; }

        [XmlElement(ElementName = "RG")]
        public string RG { get; set; }

        [XmlElement(ElementName = "UFRG")]
        public string UFRG { get; set; }

        [XmlElement(ElementName = "ORGRG")]
        public string OrgaoEmissorRG { get; set; }

        [XmlElement(ElementName = "EMRG")]
        public string DataEmissaoRG { get; set; }

        [XmlElement(ElementName = "DTN")]
        public string DataNascimento { get; set; }

        [XmlElement(ElementName = "CID")]
        public string CidadeNascimento { get; set; }

        [XmlElement(ElementName = "UFN")]
        public string UFNascimento { get; set; }

        [XmlElement(ElementName = "PAI")]
        public string NomePai { get; set; }

        [XmlElement(ElementName = "MAE")]
        public string NomeMae { get; set; }

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

        [XmlElement(ElementName = "CEL1")]
        public string Celular1 { get; set; }

        [XmlElement(ElementName = "CEL2")]
        public string Celular2 { get; set; }

        [XmlElement(ElementName = "TEL1")]
        public string Telefone1 { get; set; }

        [XmlElement(ElementName = "TEL2")]
        public string Telefone2 { get; set; }

        [XmlElement(ElementName = "IDNEXTEL")]
        public string IdNextel { get; set; }
    }
}
