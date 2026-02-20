using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.GoldenService.SolicitacaoMonitoramento
{
    public class Veiculo
    {
        public Veiculo() { }

        public Veiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresaProprietaria)
        {
            Placa = veiculo.Placa;
            Tipo = veiculo.ModeloVeicularCarga?.CodigoIntegracaoGoldenService ?? string.Empty;
            //Chassi = veiculo.Chassi;
            //RENAVAM = veiculo.Renavam;
            Marca = veiculo.Marca?.Descricao ?? string.Empty;
            Modelo = veiculo.Modelo?.Descricao ?? string.Empty;
            //Cor = veiculo.;
            //AnoFabricacao = veiculo.AnoFabricacao.ToString();
            //AnoModelo = veiculo.AnoModelo.ToString();
            //PaisEmplacamento = ;
            //CidadeEmplacamento = ;
            //UFEmplacamento = ;
            PossuiRastreador = veiculo.PossuiRastreador ? "S" : "N";

            if (veiculo.PossuiRastreador)
            {
                TecnologiaRastreador = veiculo.TecnologiaRastreador?.CodigoIntegracao;
                TipoComunicacaoRastreador = veiculo.TipoComunicacaoRastreador?.CodigoIntegracao;
                NumeroEquipamento = veiculo.NumeroEquipamentoRastreador;
            }

            if (veiculo.Tipo == "T")
            {
                if (veiculo.Proprietario?.Tipo == "F")
                    CPFProprietario = veiculo.Proprietario?.CPF_CNPJ_SemFormato;
                else
                    CNPJProprietario = veiculo.Proprietario?.CPF_CNPJ_SemFormato;

                NomeProprietario = veiculo.Proprietario?.Nome;
            }
            else
            {
                CNPJProprietario = empresaProprietaria.CNPJ;
                NomeProprietario = empresaProprietaria.RazaoSocial;
            }
        }

        [XmlElement(ElementName = "PLACA")]
        public string Placa { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "CHASSI")]
        public string Chassi { get; set; }

        [XmlElement(ElementName = "RENAVAM")]
        public string RENAVAM { get; set; }

        [XmlElement(ElementName = "MARCA")]
        public string Marca { get; set; }

        [XmlElement(ElementName = "MODELO")]
        public string Modelo { get; set; }

        [XmlElement(ElementName = "COR")]
        public string Cor { get; set; }

        [XmlElement(ElementName = "ANOFAB")]
        public string AnoFabricacao { get; set; }

        [XmlElement(ElementName = "ANOMOD")]
        public string AnoModelo { get; set; }

        [XmlElement(ElementName = "PAISEMP")]
        public string PaisEmplacamento { get; set; }

        [XmlElement(ElementName = "CIDADE")]
        public string CidadeEmplacamento { get; set; }

        [XmlElement(ElementName = "UF")]
        public string UFEmplacamento { get; set; }

        [XmlElement(ElementName = "TRACK")]
        public string PossuiRastreador { get; set; }

        [XmlElement(ElementName = "TECH")]
        public string TecnologiaRastreador { get; set; }

        [XmlElement(ElementName = "TIPCOM")]
        public string TipoComunicacaoRastreador { get; set; }

        [XmlElement(ElementName = "NREQUIP")]
        public string NumeroEquipamento { get; set; }

        [XmlElement(ElementName = "CPF")]
        public string CPFProprietario { get; set; }

        [XmlElement(ElementName = "CNPJ")]
        public string CNPJProprietario { get; set; }

        [XmlElement(ElementName = "NOME")]
        public string NomeProprietario { get; set; }
    }
}
