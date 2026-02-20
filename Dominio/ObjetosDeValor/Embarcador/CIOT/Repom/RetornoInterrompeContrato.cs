using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("interrompe_contrato")]
    public class RetornoInterrompeContrato
    {
        [XmlElement("valor_recuperado")]
        public string ValorRecuperado { get; set; }

        [XmlElement("valor_recuperado_data_vencimento")]
        public string DataVencimentoValorRecuperado { get; set; }

        [XmlElement("interrupcao_tipo")]
        public string TipoInterrupcao { get; set; }

        [XmlElement("protocolo_interrupcao")]
        public string Protocolo { get; set; }
    }
}
