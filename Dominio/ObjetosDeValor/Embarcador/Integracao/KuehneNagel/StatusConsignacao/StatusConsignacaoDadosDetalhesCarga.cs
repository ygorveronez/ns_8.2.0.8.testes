using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel
{
    public sealed class StatusConsignacaoDadosDetalhesCarga
    {
        [XmlElement(ElementName = "TotalNumberOfPackages")]
        public int NumeroTotalFilhotes { get; set; }

        [XmlElement(ElementName = "NumberOfPackagesDangerous")]
        public int NumeroFilhotesPerigosos { get; set; }

        [XmlElement(ElementName = "ContainerDetails")]
        public StatusConsignacaoDadosDetalhesContainer Container { get; set; }

        [XmlElement(ElementName = "CargoDescription")]
        public StatusConsignacaoDadosDetalhesCargaConteudo[] Conteudos { get; set; }
    }
}
