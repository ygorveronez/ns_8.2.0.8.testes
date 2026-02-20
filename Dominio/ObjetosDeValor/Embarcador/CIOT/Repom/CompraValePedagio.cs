using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("viagem")]
    public class CompraValePedagio
    {
        [XmlElement("processo_cliente_codigo")]
        public string CodigoCliente { get; set; }

        [XmlElement("filial_codigo_cliente")]
        public string CodigoFilial { get; set; }

        [XmlElement("cartao")]
        public string Cartao { get; set; }

        [XmlElement("roteiro")]
        public CompraValePedagioRoteiro Roteiro { get; set; }

        [XmlElement("motorista")]
        public CompraValePedagioMotorista Motorista { get; set; }

        [XmlElement("veiculo_placa")]
        public string PlacaVeiculo { get; set; }

        [XmlElement("veiculo_eixos")]
        public string EixosVeiculo { get; set; }

        [XmlElement("carreta_placa")]
        public string PlacaCarreta { get; set; }

        [XmlElement("carreta_eixos")]
        public string EixosCarreta { get; set; }

        [XmlElement("eixos_suspensos_ida")]
        public string EixosSuspensosIda { get; set; }

        [XmlElement("eixos_suspensos_volta")]
        public string EixosSuspensosVolta { get; set; }

        [XmlElement("centro_custo_codigo")]
        public string CodigoCentroCusto { get; set; }

        [XmlElement("centro_custo_tipo")]
        public string TipoCentroCusto { get; set; }

        [XmlElement("documentos")]
        public CompraValePedagioDocumentos Documentos { get; set; }

        [XmlElement("concessionarias")]
        public string Concessionarias { get; set; }

        [XmlElement("pracas")]
        public string Pracas { get; set; }
    }
}
