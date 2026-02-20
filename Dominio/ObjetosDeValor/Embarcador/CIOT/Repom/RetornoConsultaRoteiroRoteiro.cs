using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("roteiro")]
    public class RetornoConsultaRoteiroRoteiro
    {
        [XmlElement("roteiro_codigo")]
        public string CodigoRoteiro { get; set; }

        [XmlElement("percurso_codigo")]
        public string CodigoPercurso { get; set; }

        [XmlElement("percurso_descricao")]
        public string DescricaoPercurso { get; set; }

        [XmlElement("cidade_origem")]
        public string CidadeOrigem { get; set; }

        [XmlElement("estado_origem")]
        public string EstadoOrigem { get; set; }

        [XmlElement("cidade_destino")]
        public string CidadeDestino { get; set; }

        [XmlElement("estado_destino")]
        public string EstadoDestino { get; set; }

        [XmlElement("km_ida")]
        public string KMIda { get; set; }

        [XmlElement("km_volta")]
        public string KMVolta { get; set; }

        [XmlElement("processo_transporte_tipo")]
        public string TipoProcessoTransporte { get; set; }

        [XmlElement("roteiro_cliente_codigo")]
        public string CodigoClienteRoteiro { get; set; }
    }
}
