using System.Collections.Generic;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe
{
    [XmlRoot(ElementName = "Ctes")]
    public class Response
    {
        [XmlElement(ElementName = "Cte")]
        public List<Cte> CTes { get; set; }

        [XmlElement(ElementName = "erro")]
        public string Erro { get; set; }
    }
}
