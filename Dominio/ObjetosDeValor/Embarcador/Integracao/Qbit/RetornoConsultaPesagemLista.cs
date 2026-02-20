using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Qbit
{
    [XmlRoot("Pesagem")]
    public class RetornoConsultaPesagemLista
    {
        [XmlElement("Codigo")]
        public string Codigo { get; set; }

        [XmlElement("CodigoProduto")]
        public string CodigoProduto { get; set; }

        [XmlElement("DescricaoProduto")]
        public string DescricaoProduto { get; set; }

        [XmlElement("DataCadastro")]
        public DateTime DataCadastro { get; set; }

        [XmlElement("CodigoMotorista")]
        public string CodigoMotorista { get; set; }

        [XmlElement("NomeMotorista")]
        public string NomeMotorista { get; set; }

        [XmlElement("CPFMotorista")]
        public string CPFMotorista { get; set; }

        [XmlElement("PlacaCavalo")]
        public string PlacaCavalo { get; set; }

        [XmlElement("PlacaCarreta")]
        public string PlacaCarreta { get; set; }

        [XmlElement("PesoTara")]
        public decimal PesoTara { get; set; }

        [XmlElement("PesoBruto")]
        public decimal PesoBruto { get; set; }

        [XmlElement("PesoLiquido")]
        public decimal PesoLiquido { get; set; }

        [XmlElement("PesoNF")]
        public decimal PesoNF { get; set; }

        [XmlElement("PesoSeco")]
        public decimal PesoSeco { get; set; }

        [XmlElement("DataPesagemTara")]
        public DateTime DataPesagemTara { get; set; }

        [XmlElement("DataPesagemBruto")]
        public DateTime DataPesagemBruto { get; set; }

        [XmlElement("CodigoCliente")]
        public string CodigoCliente { get; set; }

        [XmlElement("NomeCliente")]
        public string NomeCliente { get; set; }

        [XmlElement("CPFCNPJCliente")]
        public string CPFCNPJCliente { get; set; }

        [XmlElement("CodigoTransportadora")]
        public string CodigoTransportadora { get; set; }

        [XmlElement("NomeTransportadora")]
        public string NomeTransportadora { get; set; }

        [XmlElement("CPFCNPJTransportadora")]
        public string CPFCNPJTransportadora { get; set; }

        [XmlElement("Deposito")]
        public string Deposito { get; set; }

        [XmlElement("Doca")]
        public string Doca { get; set; }

        [XmlElement("PesoDesconto")]
        public decimal PesoDesconto { get; set; }

        [XmlElement("PesoTotalEixo")]
        public decimal PesoTotalEixo { get; set; }

        [XmlElement("Status")]
        public int Status { get; set; }

        [XmlElement("DataEncerramento")]
        public DateTime DataEncerramento { get; set; }

        [XmlElement("MotivoTara")]
        public string MotivoTara { get; set; }

        [XmlElement("MotivoBruto")]
        public string MotivoBruto { get; set; }

        [XmlElement("Sacas")]
        public decimal Sacas { get; set; }

        [XmlElement("Movimento")]
        public int Movimento { get; set; }

        [XmlElement("Destino")]
        public string Destino { get; set; }

        [XmlElement("Observacao")]
        public string Observacao { get; set; }

        [XmlElement("PesoDiferenca")]
        public decimal PesoDiferenca { get; set; }

        [XmlElement("PorcentagemDiferenca")]
        public decimal PorcentagemDiferenca { get; set; }

        [XmlElement("MotivoLiberacaoDiferenca")]
        public string MotivoLiberacaoDiferenca { get; set; }

        [XmlElement("UsuarioLiberacaoDiferenca")]
        public string UsuarioLiberacaoDiferenca { get; set; }

        [XmlElement("CodigoProdutor")]
        public string CodigoProdutor { get; set; }

        [XmlElement("NomeProdutor")]
        public string NomeProdutor { get; set; }

        [XmlElement("CPFCNPJProdutor")]
        public string CPFCNPJProdutor { get; set; }

        [XmlElement("Cancelada")]
        public bool Cancelada { get; set; }

        [XmlElement("DataCancelada")]
        public DateTime DataCancelada { get; set; }

        [XmlElement("UsuarioCancelada")]
        public string UsuarioCancelada { get; set; }

        [XmlElement("MotivoCancelada")]
        public string MotivoCancelada { get; set; }

        [XmlElement("CodigoPrePesagem")]
        public string CodigoPrePesagem { get; set; }

        [XmlElement("IdExternoPrePesagem")]
        public string IdExternoPrePesagem { get; set; }

        [XmlElement("MinutaRomaneio")]
        public string MinutaRomaneio { get; set; }

        [XmlElement("Pesagem4Etapas")]
        public bool Pesagem4Etapas { get; set; }

        [XmlElement("NumeroAtual4Etapas")]
        public int NumeroAtual4Etapas { get; set; }

        [XmlElement("PesoTara1")]
        public decimal PesoTara1 { get; set; }

        [XmlElement("PesoBruto1")]
        public decimal PesoBruto1 { get; set; }

        [XmlElement("PesoTara2")]
        public decimal PesoTara2 { get; set; }

        [XmlElement("PesoBruto2")]
        public decimal PesoBruto2 { get; set; }

        [XmlElement("DataPesagemTara2")]
        public DateTime DataPesagemTara2 { get; set; }

        [XmlElement("DataPesagemBruto2")]
        public DateTime DataPesagemBruto2 { get; set; }

        [XmlElement("UsuarioPesagem1")]
        public string UsuarioPesagem1 { get; set; }

        [XmlElement("UsuarioPesagem2")]
        public string UsuarioPesagem2 { get; set; }
    }
}
