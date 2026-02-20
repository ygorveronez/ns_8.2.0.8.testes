using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("roteiro")]
    public class CompraValePedagioRoteiro
    {
        [XmlElement("roteiro_codigo")]
        public string CodigoRoteiro { get; set; }

        [XmlElement("percurso_codigo")]
        public string CodigoPercurso { get; set; }

        [XmlElement("roteiro_cliente_codigo")]
        public string CodigoRoteiroCliente { get; set; }

        [XmlElement("cidade_origem_ibge")]
        public string IBGECidadeOrigem { get; set; }

        [XmlElement("cidade_origem_cep")]
        public string CEPCidadeOrigem { get; set; }

        [XmlElement("estado_origem")]
        public string EstadoOrigem { get; set; }

        [XmlElement("cidade_destino_ibge")]
        public string IBGECidadeDestino { get; set; }

        [XmlElement("cidade_destino_cep")]
        public string CEPCidadeDestino { get; set; }

        [XmlElement("estado_destino")]
        public string EstadoDestino { get; set; }
    }
}
