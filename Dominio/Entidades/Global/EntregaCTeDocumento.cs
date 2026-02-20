namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_ENTREGA_DOC", EntityName = "EntregaCTeDocumento", Name = "Dominio.Entidades.EntregaCTeDocumento", NameType = typeof(EntregaCTeDocumento))]
    public class EntregaCTeDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EntregaCTe", Column = "ETC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EntregaCTe EntregaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentosCTE", Column = "NFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentosCTE DocumentosCTE { get; set; }

        public virtual EntregaCTeDocumento Clonar()
        {
            return (EntregaCTeDocumento)this.MemberwiseClone();
        }
    }
}
