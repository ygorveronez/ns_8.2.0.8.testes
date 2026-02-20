namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_XML", EntityName = "XMLNFSe", Name = "Dominio.Entidades.XMLNFSe", NameType = typeof(XMLNFSe))]
    public class XMLNFSe: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "NFX_XML", Type = "StringClob", NotNull = true)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "NFX_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoXMLNFSe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoXMLNFSe Tipo { get; set; }
    }
}
