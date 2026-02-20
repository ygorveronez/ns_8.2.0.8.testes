using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("autoriza_contrato")]
    public class RetornoAutorizaContrato
    {
        [XmlElement("codigo_contrato")]
        public string CodigoContrato { get; set; }

        [XmlElement("valor_final")]
        public string ValorFinal { get; set; }

        [XmlElement("data_pagamento")]
        public string DataPagamento { get; set; }

        [XmlElement("dias")]
        public string Dias { get; set; }

        [XmlElement("usuario")]
        public string Usuario { get; set; }

        [XmlElement("contrato_codigo")]
        public string ContratoCodigo { get; set; }

        [XmlElement("processo_transporte_codigo_cliente")]
        public string CodigoProcessoTransporteCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

        [XmlElement("ocorrencia_codigo")]
        public string CodigoOcorrencia { get; set; }

        [XmlElement("ocorrencia_descricao")]
        public string DescricaoOcorrencia { get; set; }
    }
}
