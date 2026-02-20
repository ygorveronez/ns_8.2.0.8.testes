using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class OrdemTransporteDadosEnvioDetalhesCarga
    {
        [XmlElement(ElementName = "PackageType", Order = 1)]
        public string Tipo { get; set; }

        [XmlElement(ElementName = "ContainerNumber", Order = 2)]
        public string NumeroContainer { get; set; }

        [XmlElement(ElementName = "SealNumber", Order = 3)]
        public string NumeroLacre { get; set; }

        [XmlElement(ElementName = "NumberOfPackages", Order = 4)]
        public string NumeroFilhotes { get; set; }

        [XmlElement(ElementName = "GrossWeight", Order = 5)]
        public decimal PesoBruto { get; set; }

        [XmlElement(ElementName = "Volume", Order = 6)]
        public string Volume { get; set; }

        [XmlElement(ElementName = "Content", Order = 7)]
        public OrdemTransporteDadosEnvioDetalhesCargaConteudo[] Conteudos { get; set; }
    }
}
