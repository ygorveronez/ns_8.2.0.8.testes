using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_bancarios")]
    public class RetornoQuitacaoContratoFreteProcessoTransporteContratadoDadosBancarios
    {
        [XmlElement("banco_codigo")]
        public string CodigoBanco { get; set; }

        [XmlElement("banco_descricao")]
        public string DescricaoBanco { get; set; }

        [XmlElement("agencia_codigo")]
        public string CodigoAgencia { get; set; }

        [XmlElement("agencia_digito")]
        public string DigitoAgencia { get; set; }

        [XmlElement("conta_corrente")]
        public string ContaCorrente { get; set; }

        [XmlElement("conta_corrente_dv")]
        public string DigitoVerificadorContaCorrente { get; set; }

        [XmlElement("titular_cnpj_cpf")]
        public string CPFCNPJTitular { get; set; }

        [XmlElement("titular_nome")]
        public string NomeTitular { get; set; }

        [XmlElement("endereco")]
        public string Endereco { get; set; }

        [XmlElement("cidade_descricao")]
        public string DescricaoCidade { get; set; }

        [XmlElement("estado")]
        public string Estado { get; set; }

        [XmlElement("inss")]
        public string INSS { get; set; }

        [XmlElement("sistema_agregado")]
        public string SistemaAgregado { get; set; }

        [XmlElement("escrituracao_ctrc")]
        public string EscrituracaoCTRC { get; set; }

        [XmlElement("permite_quitacao_contrato")]
        public string PermiteQuitacaoContrato { get; set; }

        [XmlElement("utiliza_emissao_modelo_ctrc")]
        public string UtilizaEmissaoModeloCTRC { get; set; }

        [XmlElement("permite_prestacao_contas")]
        public string PermitePrestacaoContas { get; set; }

        [XmlElement("protocolo_envio_documentos")]
        public string ProtocoloEnvioDocumentos { get; set; }

        [XmlElement("tipo_contratado")]
        public string TipoContratado { get; set; }

        [XmlElement("emite_cte")]
        public string EmiteCTe { get; set; }

        [XmlElement("contratado_motorista_cpf")]
        public string CPFMotorista { get; set; }

        [XmlElement("contratado_motorista_nome")]
        public string NomeMotorista { get; set; }

    }
}
