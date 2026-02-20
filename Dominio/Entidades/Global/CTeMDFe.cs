namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_CTE", EntityName = "CTeMDFe", Name = "Dominio.Entidades.CTeMDFe", NameType = typeof(CTeMDFe))]
    public class CTeMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MunicipioDescarregamentoMDFe", Column = "MDD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.MunicipioDescarregamentoMDFe MunicipioDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "MDC_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }
    }
}
