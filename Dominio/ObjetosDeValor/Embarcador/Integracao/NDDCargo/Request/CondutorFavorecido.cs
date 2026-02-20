using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class CondutorFavorecido
    {
        [XmlElement(ElementName = "cpf")]
        public string Cpf { get; set; }
    }
}
