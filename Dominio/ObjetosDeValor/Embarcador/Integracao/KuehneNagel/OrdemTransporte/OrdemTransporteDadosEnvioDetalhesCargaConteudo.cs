using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEnvioDetalhesCargaConteudo
    {
        [XmlElement(ElementName = "CargoDescription", Order = 1)]
        public string Descricao { get; set; }

        [XmlElement(ElementName = "MarksAndNumbers", Order = 2)]
        public string MarcasENumeros { get; set; }
    }
}
