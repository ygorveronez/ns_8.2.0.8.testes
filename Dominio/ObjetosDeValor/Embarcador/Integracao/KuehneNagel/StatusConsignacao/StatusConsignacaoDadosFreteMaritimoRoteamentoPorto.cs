using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosFreteMaritimoRoteamentoPorto
    {
        [XmlElement(ElementName = "Location")]
        public string Localizacao { get; set; }

        [XmlElement(ElementName = "DateTime")]
        public DataHora Data { get; set; }
    }
}
