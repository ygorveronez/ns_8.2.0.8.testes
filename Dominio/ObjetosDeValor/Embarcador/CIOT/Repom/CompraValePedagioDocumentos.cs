using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("documentos")]
    public class CompraValePedagioDocumentos
    {
        [XmlElement("documento")]
        public CompraValePedagioDocumento[] Documento { get; set; }
    }
}
