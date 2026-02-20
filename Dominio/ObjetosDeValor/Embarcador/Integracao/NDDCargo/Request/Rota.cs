using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request
{
    public class Rota
    {
        [XmlElement(ElementName = "rotaERP")]
        public string RotaERP { get; set; }

        [XmlElement(ElementName = "informacoes")]
        public InformacoesRota Informacoes { get; set; }
    }
}
