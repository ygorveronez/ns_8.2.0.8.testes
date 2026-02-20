using System.Xml.Serialization;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    [XmlRoot(ElementName = "EspelhamentoPendenteVeiculo")]
    public class EspelhamentoPendenteVeiculo
    {
        [XmlElement(ElementName = "veiID")]
        public string VeiID { get; set; }
        [XmlElement(ElementName = "placa")]
        public string Placa { get; set; }
        [XmlElement(ElementName = "prop")]
        public string Prop { get; set; }
        [XmlElement(ElementName = "validade")]
        public string Validade { get; set; }
        [XmlElement(ElementName = "propCancelamento")]
        public string PropCancelamento { get; set; }
        [XmlElement(ElementName = "alteraValidade")]
        public string AlteraValidade { get; set; }
        [XmlElement(ElementName = "historicoAPartirDe")]
        public string HistoricoAPartirDe { get; set; }
    }

    [XmlRoot(ElementName = "ResponseEspelhamentoPendenteVeiculo")]
    public class ResponseEspelhamentoPendenteVeiculo
    {
        [XmlElement(ElementName = "EspelhamentoPendenteVeiculo")]
        public List<EspelhamentoPendenteVeiculo> EspelhamentoPendenteVeiculo { get; set; }
    }
}
