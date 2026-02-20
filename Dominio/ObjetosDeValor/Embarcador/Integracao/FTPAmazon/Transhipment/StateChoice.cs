using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon
{
    [XmlRoot(ElementName = "stateChoice", Namespace = "")]
    public sealed class StateChoice
    {
        [XmlElement(ElementName = "stateProvince")]
        public string StateProvince { get; set; }

        [XmlText]
        public string TextValue { get; set; }

        public string GetValue()
        {
            return StateProvince ?? TextValue;
        }

    }
}
