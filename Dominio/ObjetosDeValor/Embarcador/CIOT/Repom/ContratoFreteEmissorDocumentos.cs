using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("integracao_emissor_documentos")]
    public class ContratoFreteEmissorDocumentos
    {
        [XmlElement("cnpj_integrador")]
        public string CNPJIntegrador { get; set; }
    }
}
