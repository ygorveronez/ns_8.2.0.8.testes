using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit
{
    public class GswReceiverRequest
    {
        private string _inputXML;

        [XmlElement(ElementName = "CodigoAcao")]
        public string CodigoAcao { get; set; }

        [XmlElement(ElementName = "InputXML")]
        public string InputXML
        {
            get => _inputXML;
            set => _inputXML = value?.Replace("<![CDATA[", "").Replace("]]>", "");
        }

        [XmlElement(ElementName = "TipoNFe")]
        public string TipoNFe { get; set; }
    }
}
