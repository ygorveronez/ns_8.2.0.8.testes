using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_operacionais")]
    public class ContratoFreteDadosOperacionais
    {
        [XmlElement("operacao_codigo")]
        public string CodigoOperacao { get; set; }

        [XmlElement("roteiro_codigo")]
        public string CodigoRoteiro { get; set; }

        [XmlElement("percurso_codigo")]
        public string CodigoPercurso { get; set; }

        [XmlElement("solicitacao_roteiro_codigo")]
        public string CodigoSolicitacaoRoteiro { get; set; }

        [XmlElement("roteiro_cliente_codigo")]
        public string CodigoRoteiroCliente { get; set; }

        [XmlElement("roteiro_ida_volta")]
        public string RoteiroIdaVolta { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilialCliente { get; set; }

        [XmlElement("processo_cliente_codigo")]
        public string CodigoProcessoCliente { get; set; }

        [XmlElement("cartao_codigo")]
        public string CodigoCartao { get; set; }

        [XmlElement("usuario")]
        public string Usuario { get; set; }

        [XmlElement("cep_origem")]
        public string CEPOrigem { get; set; }

        [XmlElement("cep_destino")]
        public string CEPDestino { get; set; }

        [XmlElement("distancia_percorrida")]
        public string DistanciaPercorrida { get; set; }

        [XmlElement("roteiro_pagamento_pedagio")]
        public string RoteiroPagamentoPedagio { get; set; }
    }
}
