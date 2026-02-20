namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_XML_MDFE", EntityName = "RetornoXMLMDFe", Name = "Dominio.Entidades.RetornoXMLMDFe", NameType = typeof(RetornoXMLMDFe))]
    public class RetornoXMLMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RXM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "RXM_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }
    }
}
