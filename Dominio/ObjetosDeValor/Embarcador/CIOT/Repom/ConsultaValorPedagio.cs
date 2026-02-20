using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("roteiro")]
    public class ConsultaValorPedagio
    {
        [XmlElement("roteiro_codigo")]
        public string CodigoRoteiro { get; set; }

        [XmlElement("percurso_codigo")]
        public string CodigoPercurso { get; set; }

        [XmlElement("roteiro_cliente_codigo")]
        public string CodigoRoteiroCliente { get; set; }

        [XmlElement("numero_eixos")]
        public string NumeroEixos { get; set; }

        [XmlElement("eixos_suspensos_ida")]
        public string EixosSuspensosIda { get; set; }

        [XmlElement("eixos_suspensos_volta")]
        public string EixosSuspensosVolta { get; set; }
    }
}
