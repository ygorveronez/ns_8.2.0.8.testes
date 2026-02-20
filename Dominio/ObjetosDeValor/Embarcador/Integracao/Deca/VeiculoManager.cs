using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Deca
{
    [XmlRoot(ElementName = "vehicleManagement", Namespace = "")]
    public class VeiculoManager
    {
        [XmlElement(ElementName = "vehicle", Namespace = "")]
        public Veiculo Veiculo { get; set; }

        [XmlElement(ElementName = "Condutor", Namespace = "")]
        public string Condutor { get; set; }

        [XmlElement(ElementName = "PlacaVeiculo", Namespace = "")]
        public string PlacaVeiculo { get; set; }

		[XmlElement(ElementName = "PlacaCarreta", Namespace = "")]
		public string PlacaCarreta { get; set; }

		[XmlElement(ElementName = "Documento", Namespace = "")]
        public string Documento { get; set; }
    }
}
