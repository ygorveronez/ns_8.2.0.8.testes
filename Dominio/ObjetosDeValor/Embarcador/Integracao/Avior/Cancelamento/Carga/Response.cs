using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga
{
    [XmlRoot(ElementName = "Fretes")]
    public class Response
    {
        [XmlElement(ElementName = "Frete")]
        public List<Frete> Fretes { get; set; }

        [XmlElement(ElementName = "erro")]
        public string Erro { get; set; }
    }
}
