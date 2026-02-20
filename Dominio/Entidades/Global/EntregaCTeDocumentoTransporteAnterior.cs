namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_ENTREGA_DOC_TRANSP_ANT", EntityName = "EntregaCTeDocumentoTransporteAnterior", Name = "Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior", NameType = typeof(EntregaCTeDocumentoTransporteAnterior))]
    public class EntregaCTeDocumentoTransporteAnterior : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EntregaCTe", Column = "ETC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EntregaCTe EntregaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoDeTransporteAnteriorCTe", Column = "CSU_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoDeTransporteAnteriorCTe DocumentoTransporteAnterior { get; set; }

        public virtual EntregaCTeDocumento Clonar()
        {
            return (EntregaCTeDocumento)this.MemberwiseClone();
        }
    }
}
