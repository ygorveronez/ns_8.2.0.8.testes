using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class InfRota
    {
        [XmlElement(ElementName = "categoriaPedagio")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores.CategoriaPedagio CategoriaPedagio { get; set; }

        [XmlElement(ElementName = "rota")]
        public Rota Rota { get; set; }
    }
}
