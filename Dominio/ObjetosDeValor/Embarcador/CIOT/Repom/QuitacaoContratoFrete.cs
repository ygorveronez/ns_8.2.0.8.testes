using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("processo_transporte")]
    public class QuitacaoContratoFrete
    {
        [XmlElement("cliente_codigo")]
        public string CodigoCliente { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("processo_cliente_filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

        [XmlElement("processo_transporte_codigo")]
        public string CodigoProcessoTransporte { get; set; }

        /// <summary>
        /// Código da Filial onde o contrato esta sendo quitado
        /// </summary>
        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilialQuitacao { get; set; }

        [XmlElement("peso_entrega")]
        public string PesoEntrega { get; set; }

        [XmlElement("avarias")]
        public string Avarias { get; set; }

        [XmlElement("ocorrencias")]
        public string Ocorrencias { get; set; }

        [XmlElement("data_prevista_pagamento")]
        public string DataPrevistaPagamento { get; set; }

        [XmlElement("plano_pagamento")]
        public string PlanoPagamento { get; set; }

        /// <summary>
        /// 0 quitação sem processo de rateio  
        /// 1 quitação com processo de rateio e baixa de documento 
        /// </summary>
        [XmlElement("quitacao_tipo")]
        public string TipoQuitacao { get; set; }
    }
}
