namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_XML", EntityName = "XMLMDFe", Name = "Dominio.Entidades.XMLMDFe", NameType = typeof(XMLMDFe))]
    public class XMLMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "MDX_XML", Type = "StringClob", NotNull = true)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MDX_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoXMLMDFe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoXMLMDFe Tipo { get; set; }
    }
}
