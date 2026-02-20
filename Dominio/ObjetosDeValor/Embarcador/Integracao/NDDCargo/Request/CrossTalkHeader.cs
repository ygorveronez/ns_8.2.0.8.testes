using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    [XmlRoot(ElementName = "CrossTalk_Header")]
    public class CrossTalkHeader
    {
        [XmlElement(ElementName = "ProcessCode")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores.ProcessCode ProcessCode { get; set; }

        [XmlElement(ElementName = "MessageType")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores.MessageType MessageType { get; set; }

        [XmlElement(ElementName = "ExchangePattern")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores.ExchangePattern ExchangePattern { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public DateTime DateTime { get; set; }

        [XmlElement(ElementName = "EnterpriseId")]
        public string EnterpriseId { get; set; }

        [XmlElement(ElementName = "Token")]
        public string Token { get; set; }
    }

}
