using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("documentos_integrados")]
    public class ContratoFreteDocumentosIntegrados
    {
        [XmlArray("conhecimentos"), XmlArrayItem("conhecimento")]
        public ContratoFreteDocumentosIntegradosConhecimento[] CTes { get; set; }
    }
}
