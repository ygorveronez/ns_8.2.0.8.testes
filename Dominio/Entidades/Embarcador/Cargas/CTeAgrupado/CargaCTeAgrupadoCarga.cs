namespace Dominio.Entidades.Embarcador.Cargas.CTeAgrupado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CTE_AGRUPADO_CARGA", EntityName = "CargaCTeAgrupadoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCarga", NameType = typeof(CargaCTeAgrupadoCarga))]
    public class CargaCTeAgrupadoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupado", Column = "CCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado CargaCTeAgrupado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        /// <summary>
        /// CT-e gerado para esta carga
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTeAgrupadoCTe", Column = "CCE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe CargaCTeAgrupadoCTe { get; set; }
    }
}
